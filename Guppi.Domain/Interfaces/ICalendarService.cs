using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities;

namespace Guppi.Domain.Interfaces
{
    public interface ICalendarService
    {
        Task<IEnumerable<CalendarEvent>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);

        void Logout();
    }
}
