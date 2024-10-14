using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Calendar;

namespace Guppi.Core.Interfaces.Providers
{
    public interface ICalendarProvider
    {
        Task<IList<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate);

        Task<string> Logout();

        string Name { get; }
    }
}
