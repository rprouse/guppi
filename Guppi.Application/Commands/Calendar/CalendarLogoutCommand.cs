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
        private readonly ICalendarService _calendarService;

        public CalendarLogoutCommandHandler(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<Unit> Handle(CalendarLogoutCommand request, CancellationToken cancellationToken)
        {
            _calendarService.Logout();
            return await Unit.Task;
        }
    }
}
