using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Calendar
{
    public sealed class CalendarEventsQuery : IRequest<IEnumerable<EventDto>>
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }

    public sealed class CalendarEventsQueryHandler : IRequestHandler<CalendarEventsQuery, IEnumerable<EventDto>>
    {
        private readonly ICalendarService _calendarService;

        public CalendarEventsQueryHandler(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<IEnumerable<EventDto>> Handle(CalendarEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _calendarService.GetCalendarEvents(request.MinDate, request.MaxDate);
            return events.Select(e => new EventDto { Start = e.Start, End = e.End, Summary = e.Summary });
        }
    }
}
