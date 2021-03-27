using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Weather
{
    internal sealed class WeatherService : IWeatherService
    {
        const string Command = "weather";
        const string Name = "Weather";
        private readonly IHttpRestService _restService;
        WeatherConfiguration _configuration;

        public WeatherService(IHttpRestService restService)
        {
            _configuration = Configuration.Load<WeatherConfiguration>(Command);
            _restService = restService;
        }

        public void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the OpenWeather API key and your location");
        }

        public async Task<Domain.Entities.Weather.WeatherForecast> GetWeatherData()
        {
            if (!Configured)
            {
                throw new UnconfiguredException("Please configure the weather provider");
            }

            var weather = await _restService.GetData<WeatherResponse>($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
            return weather.GetWeather();
        }

        private bool Configured => _configuration.Configured;
    }
}
