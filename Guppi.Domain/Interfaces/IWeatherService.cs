using System.Threading.Tasks;
using Guppi.Domain.Entities.Weather;

namespace Guppi.Domain.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherForecast> GetWeatherData();

        void Configure();
    }
}
