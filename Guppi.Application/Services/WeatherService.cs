using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Application.Services.Weather;
using Guppi.Domain.Entities.Weather;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

internal sealed class WeatherService : IWeatherService
{
    private readonly IHttpRestService _restService;

    public WeatherService(IHttpRestService restService)
    {
        _restService = restService;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<WeatherConfiguration>("weather");
        configuration.RunConfiguration("Weather", "Enter the OpenWeather API key and your location");
    }

    public async Task<WeatherForecast> GetWeather()
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
