using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Calendar;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Calendar
{
    public sealed class CalendarEventsQuery : IRequest<IEnumerable<Event>>
    {
        public DateTime? MinDate { get; init; }
        public DateTime? MaxDate { get; init; }
    }

    internal sealed class CalendarEventsQueryHandler : IRequestHandler<CalendarEventsQuery, IEnumerable<Event>>
    {
        private readonly IEnumerable<ICalendarService> _calendarServices;

        public CalendarEventsQueryHandler(IEnumerable<ICalendarService> calendarServices)
        {
            _calendarServices = calendarServices;
        }

        public async Task<IEnumerable<Event>> Handle(CalendarEventsQuery request, CancellationToken cancellationToken)
        {
            var events = new List<Event>();
            foreach(var service in _calendarServices)
            {
                events.AddRange(await service.GetCalendarEvents(request.MinDate, request.MaxDate));
            }
            return events.OrderBy(e => e.Start);
        }
    }
}
