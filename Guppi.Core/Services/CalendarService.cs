using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Entities.Calendar;

namespace Guppi.Core.Services;

internal sealed class CalendarService : ICalendarService
{
    private readonly IEnumerable<Interfaces.ICalendarService> _calendarServices;

    public CalendarService(IEnumerable<Interfaces.ICalendarService> calendarServices)
    {
        _calendarServices = calendarServices;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<CalendarConfiguration>("Calendar");
        configuration.SetCalendarServices(_calendarServices);
        configuration.RunConfiguration("Calendars", "Configure your calendars.");
    }

    public async Task Logout()
    {
        // TODO: Return the string
        foreach (var service in _calendarServices)
            await service.Logout();
    }

    public async Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
    {
        var configuration = Configuration.Load<CalendarConfiguration>("Calendar");
        if (!configuration.Enabled)
            throw new UnconfiguredException("Please configure calendars");

        var events = new List<Event>();
        foreach (var service in _calendarServices.Where(c => configuration.EnabledCalendars.Any(e => e.Enabled && e.Name == c.Name)))
        {
            events.AddRange(await service.GetCalendarEvents(minDate, maxDate));
        }
        return events.OrderBy(e => e.Start);
    }
}
