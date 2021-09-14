using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Calendar
{
    public sealed class CalendarLogoutCommand : IRequest
    {
    }

    internal sealed class CalendarLogoutCommandHandler : IRequestHandler<CalendarLogoutCommand>
    {
        private readonly IEnumerable<ICalendarService> _calendarServices;

        public CalendarLogoutCommandHandler(IEnumerable<ICalendarService> calendarServices)
        {
            _calendarServices = calendarServices;
        }

        public async Task<Unit> Handle(CalendarLogoutCommand request, CancellationToken cancellationToken)
        {
            Parallel.ForEach(_calendarServices, service =>
                service.Logout()
            );
            return await Unit.Task;
        }
    }
}
