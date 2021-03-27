using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using MediatR;

namespace Guppi.Application.Commands.Weather
{
    public sealed class ConfigureWeatherCommand : IRequest
    {
    }

    internal sealed class ConfigureWeatherCommandHandler : IRequestHandler<ConfigureWeatherCommand>
    {
        public async Task<Unit> Handle(ConfigureWeatherCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<WeatherConfiguration>("weather");
            configuration.RunConfiguration("Weather", "Enter the OpenWeather API key and your location");
            return await Unit.Task;
        }
    }
}
