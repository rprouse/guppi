using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Weather;

namespace Guppi.Application.Services;

public interface IWeatherService
{
    void Configure();

    Task<WeatherForecast> GetWeather();

    Task<WeatherForecast> GetWeather(string latitude, string longitude);

    Task<IEnumerable<Domain.Entities.Weather.Location>> GetLocations(string search);
}
