using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Weather;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queryies.Weather
{
    public sealed class WeatherQuery : IRequest<WeatherForecast>
    {
    }

    internal sealed class WeatherQueryHandler : IRequestHandler<WeatherQuery, WeatherForecast>
    {
        private readonly IWeatherService _weatherService;

        public WeatherQueryHandler(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        
        public async Task<WeatherForecast> Handle(WeatherQuery request, CancellationToken cancellationToken) =>
            await _weatherService.GetWeatherData();
    }
}
