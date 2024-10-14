using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core;
using Guppi.Core.Exceptions;
using Guppi.Core.Entities.Calendar;
using Guppi.Core.Interfaces;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace Guppi.Core.Providers.Calendar;

internal sealed class ICalCalendarProvider(IHttpRestProvider httpService) : ICalendarProvider
{
    private readonly IHttpRestProvider _httpService = httpService;
    public string Name => "iCal Calendars";

    public async Task<IList<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
    {
        IDateTime start = minDate.HasValue ?
            new CalDateTime(minDate.Value, "Local") :
            CalDateTime.Now;

        IDateTime end = maxDate.HasValue ?
            new CalDateTime(maxDate.Value, "Local") :
            CalDateTime.Now.AddDays(1);

        var configuration = Configuration.Load<Core.Configurations.CalendarConfiguration>("Calendar");
        if (!configuration.Enabled)
            throw new UnconfiguredException("Please configure calendars");

        var events = new List<Event>();
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
                MeetingUrl = MeetingUrl(e),
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
                    MeetingUrl = MeetingUrl(e),
                };
            });

        events.AddRange(recurringEvents);
        return events;
    }

    public Task<string> Logout() =>
        Task.FromResult("Log out of ICal not required");

    private static string MeetingUrl(CalendarEvent e)
    {
        // URL is in the location
        if (e.Location?.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ?? false)
            return e.Location;

        // URL is in the description
        if (e.Location?.Contains("Teams") == true)
        {
            int start = e.Description.IndexOf("https://teams.microsoft.com/l/meetup-join/");
            if (start > -1)
            {
                int end = e.Description.IndexOf('>', start);
                string url = e.Description[start..end];
                return url;
            }
        }

        // Couldn't find the URL
        return null;
    }
}
