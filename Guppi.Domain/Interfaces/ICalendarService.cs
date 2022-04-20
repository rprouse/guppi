using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Calendar;

namespace Guppi.Domain.Interfaces
{
    public interface ICalendarService
    {
        Task<IList<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);

        Task<string> Logout();
    }
}
