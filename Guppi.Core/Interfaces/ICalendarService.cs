using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Calendar;

namespace Guppi.Core.Interfaces;

public interface ICalendarService
{
    void Configure();
    Task Logout();
    Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);
}
