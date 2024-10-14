using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Calendar;

namespace Guppi.Core.Services;

public interface ICalendarService
{
    void Configure();
    Task Logout();
    Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);
}
