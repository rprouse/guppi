using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using MediatR;

namespace Guppi.Application.Commands.Strava
{
    public record ConfigureStravaCommand : IRequest
    {
    }

    internal sealed class ConfigureStravaCommandHandler : IRequestHandler<ConfigureStravaCommand>
    {
        public async Task<Unit> Handle(ConfigureStravaCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<StravaConfiguration>("strava");
            configuration.RunConfiguration("Strava", "Enter the Strava Client Id and Secret");
            return await Unit.Task;
        }
    }
}
