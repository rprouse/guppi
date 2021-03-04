using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Weather
{
    public sealed class ConfigureWeatherCommand : IRequest
    {
    }

    internal sealed class ConfigureWeatherCommandHandler : IRequestHandler<ConfigureWeatherCommand>
    {
        private readonly IWeatherService _weatherService;

        public ConfigureWeatherCommandHandler(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<Unit> Handle(ConfigureWeatherCommand request, CancellationToken cancellationToken)
        {
            _weatherService.Configure();
            return await Unit.Task;
        }
    }
}
