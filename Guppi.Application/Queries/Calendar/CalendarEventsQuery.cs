using System;
using System.Collections.Generic;
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
        private readonly ICalendarService _calendarService;

        public CalendarEventsQueryHandler(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<IEnumerable<Event>> Handle(CalendarEventsQuery request, CancellationToken cancellationToken) =>
            await _calendarService.GetCalendarEvents(request.MinDate, request.MaxDate);
    }
}
