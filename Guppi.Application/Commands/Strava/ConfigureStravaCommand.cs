using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Strava
{
    public record ConfigureStravaCommand : IRequest
    {
    }

    internal sealed class ConfigureStravaCommandHandler : IRequestHandler<ConfigureStravaCommand>
    {
        private readonly IStravaService _stravaService;

        public ConfigureStravaCommandHandler(IStravaService stravaService)
        {
            _stravaService = stravaService;
        }

        public async Task<Unit> Handle(ConfigureStravaCommand request, CancellationToken cancellationToken)
        {
            _stravaService.Configure();
            return await Unit.Task;
        }
    }
}
