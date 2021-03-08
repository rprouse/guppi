using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Weather
{
    internal sealed class WeatherService : HttpService, IWeatherService
    {
        const string Command = "weather";
        const string Name = "Weather";

        WeatherConfiguration _configuration;

        public WeatherService()
        {
            _configuration = Configuration.Load<WeatherConfiguration>(Command);
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

            var weather = await GetData<WeatherResponse>($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
            return weather.GetWeather();
        }

        private bool Configured => _configuration.Configured;
    }
}
