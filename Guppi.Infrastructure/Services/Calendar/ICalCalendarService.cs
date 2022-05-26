using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.Calendar;
using Guppi.Domain.Interfaces;
using Ical.Net.DataTypes;

namespace Guppi.Infrastructure.Services.Calendar
{
    internal sealed class ICalCalendarService : ICalendarService
    {
        private readonly IHttpRestService _httpService;
        public string Name => "iCal Calendars";

        public ICalCalendarService(IHttpRestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<IList<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
        {
            IDateTime start = minDate.HasValue ?
                new CalDateTime(minDate.Value, "Local") :
                CalDateTime.Now;

            IDateTime end = maxDate.HasValue ?
                new CalDateTime(maxDate.Value, "Local") :
                CalDateTime.Now.AddDays(1);

            var configuration = Configuration.Load<Application.Configurations.CalendarConfiguration>("Calendar");
            if (!configuration.Enabled)
                throw new UnconfiguredException("Please configure calendars");

            List<Event> events = new List<Event>();
            foreach (var icalUrl in configuration.ICalUrls)
            {
                var e = await GetEventsFromCalendar(icalUrl, start, end);
                events.AddRange(e);
            }
            return events;
        }

        private async Task<IList<Event>> GetEventsFromCalendar(string icalUrl, IDateTime start, IDateTime end)
        {
            icalUrl = icalUrl.Replace("webcal", "https", StringComparison.InvariantCultureIgnoreCase);
            string ical = await _httpService.GetStringAsync(icalUrl);

            var calendar = Ical.Net.Calendar.Load(ical);

            var events = calendar.Events
                .Where(e =>
                    e.RecurrenceRules?.Count == 0 &&
                    e.Start.GreaterThanOrEqual(start) &&
                    e.Start.LessThan(end)
                )
                .Select(e => new Event
                {
                    Summary = e.Summary,
                    Start = e.Start.ToTimeZone("Local").Value,
                    End = e.End.ToTimeZone("Local").Value,
                })
                .ToList();

            var recurringEvents = calendar.Events
                .Where(e =>
                    e.RecurrenceRules?.Count > 0 &&
                    e.GetOccurrences(start, end).Count > 0
                )
                .Select(e =>
                {
                    var period = e.GetOccurrences(start, end)
                        .First()
                        .Period;

                    return new Event
                    {
                        Summary = e.Summary,
                        Start = period.StartTime.ToTimeZone("Local").Value,
                        End = period.EndTime.ToTimeZone("Local").Value,
                    };
                });

            events.AddRange(recurringEvents);
            return events;
        }

        public Task<string> Logout() =>
            Task.FromResult("Log out of ICal not required");
    }
}
