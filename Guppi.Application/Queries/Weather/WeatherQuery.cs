using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.Weather;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Weather
{
    public sealed class WeatherQuery : IRequest<WeatherForecast>
    {
    }

    internal sealed class WeatherQueryHandler : IRequestHandler<WeatherQuery, WeatherForecast>
    {
        private readonly IHttpRestService _restService;

        public WeatherQueryHandler(IHttpRestService restService)
        {
            _restService = restService;
        }

        public async Task<WeatherForecast> Handle(WeatherQuery request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<WeatherConfiguration>("weather");
            if (!configuration.Configured)
            {
                throw new UnconfiguredException("Please configure the weather provider");
            }

            var weather = await _restService.GetData<WeatherResponse>($"http://api.openweathermap.org/data/2.5/onecall?lat={configuration.Latitude}&lon={configuration.Longitude}&appid={configuration.ApiKey}");
            return weather.GetWeather();
        }
    }
}
