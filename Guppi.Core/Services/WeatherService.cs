using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Services.Weather;
using Guppi.Core.Entities.Weather;
using Guppi.Core.Interfaces.Providers;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

internal sealed class WeatherService(IHttpRestProvider restService) : IWeatherService
{
    private readonly IHttpRestProvider _restService = restService;
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

        var weather = await _restService.GetData<WeatherResponse>($"https://api.openweathermap.org/data/3.0/onecall?lat={latitude}&lon={longitude}&appid={_configuration.ApiKey}");
        return weather.GetWeather();
    }

    public async Task<IEnumerable<Entities.Weather.Location>> GetLocations(string search)
    {
        if (!_configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the weather provider");
        }

        var locations = await _restService.GetData<IEnumerable<LocationResponse>>($"https://api.openweathermap.org/geo/1.0/direct?q={search}&limit=10&appid={_configuration.ApiKey}");
        return locations.Select(r => r.GetLocation());
    }
}
