using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Entities.Weather;

namespace Guppi.Core.Interfaces.Services;

public interface IWeatherService
{
    void Configure();

    Task<WeatherForecast> GetWeather();

    Task<WeatherForecast> GetWeather(string latitude, string longitude);

    Task<IEnumerable<Location>> GetLocations(string search);
}
