using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Calendar;

namespace Guppi.Domain.Interfaces
{
    public interface ICalendarService
    {
        Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);

        void Logout();
    }
}
