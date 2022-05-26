using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Calendar
{
    public sealed class ConfigureCalendarCommand : IRequest
    {
    }

    public sealed class ConfigureCalendarCommandHandler : IRequestHandler<ConfigureCalendarCommand>
    {
        private readonly IEnumerable<ICalendarService> _calendarServices;

        public ConfigureCalendarCommandHandler(IEnumerable<ICalendarService> calendarServices)
        {
            _calendarServices = calendarServices;
        }

        public async Task<Unit> Handle(ConfigureCalendarCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<CalendarConfiguration>("Calendar");
            configuration.SetCalendarServices(_calendarServices);
            configuration.RunConfiguration("Calendars", "Configure your calendars.");
            return await Unit.Task;
        }
    }
}
