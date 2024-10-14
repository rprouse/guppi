using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Application.Services.Weather;
using Guppi.Domain.Entities.Weather;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

internal sealed class WeatherService(IHttpRestService restService) : IWeatherService
{
    private readonly IHttpRestService _restService = restService;
    private readonly WeatherConfiguration _configuration = Configuration.Load<WeatherConfiguration>("weather");

    public void Configure()
    {
        _configuration.RunConfiguration("Weather", "Enter the OpenWeather API key and your location");
    }

    public async Task<WeatherForecast> GetWeather()
    {
        if (!_configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the weather provider");
        }
        return await GetWeather(_configuration.Latitude, _configuration.Longitude);
    }

    public async Task<WeatherForecast> GetWeather(string latitude, string longitude)
    {
        if (!_configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the weather provider");
        }

        var weather = await _restService.GetData<WeatherResponse>($"https://api.openweathermap.org/data/2.5/onecall?lat={latitude}&lon={longitude}&appid={_configuration.ApiKey}");
        return weather.GetWeather();
    }

    public async Task<IEnumerable<Domain.Entities.Weather.Location>> GetLocations(string search)
    {
        if (!_configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the weather provider");
        }

        var locations = await _restService.GetData<IEnumerable<LocationResponse>>($"https://api.openweathermap.org/geo/1.0/direct?q={search}&limit=10&appid={_configuration.ApiKey}");
        return locations.Select(r => r.GetLocation());
    }
}
