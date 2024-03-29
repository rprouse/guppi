using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Calendar;

namespace Guppi.Application.Services;

public interface ICalendarService
{
    void Configure();
    Task Logout();
    Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);
}
