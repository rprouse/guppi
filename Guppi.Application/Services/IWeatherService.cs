using System.Threading.Tasks;
using Guppi.Domain.Entities.Weather;

namespace Guppi.Application.Services;

public interface IWeatherService
{
    void Configure();
    Task<WeatherForecast> GetWeather();
}
