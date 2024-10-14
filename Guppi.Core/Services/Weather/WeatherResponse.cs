using System.Linq;
using Guppi.Core.Extensions;
using Guppi.Domain.Entities.Weather;

namespace Guppi.Core.Services.Weather
{
#pragma warning disable IDE1006 // Naming Styles
    internal class WeatherResponse
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public Current current { get; set; }
        public Hourly[] hourly { get; set; }
        public Daily[] daily { get; set; }

        public WeatherForecast GetWeather() =>
            new WeatherForecast
            {
                Current = current.GetCurrentWeather(),
                Hourly = hourly.Select(h => h.GetHourlyWeather()).ToArray(),
                Daily = daily.Select(d => d.GetDailyWeather()).ToArray()
            };
    }

    internal class Current
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public float wind_gust { get; set; }
        public Weather[] weather { get; set; }

        public CurrentWeather GetCurrentWeather() =>
            new CurrentWeather
            {
                DateTime = dt.UnixTimeStampToDateTime(),
                Sunrise = sunrise.UnixTimeStampToDateTime(),
                Sunset = sunset.UnixTimeStampToDateTime(),
                Temperature = temp.KalvinToCelcius(),
                FeelsLike = feels_like.KalvinToCelcius(),
                Pressure = pressure,
                Humidity = humidity,
                Clouds = clouds,
                WindSpeed = wind_speed,
                WindDirection = wind_deg,
                Description = weather.FirstOrDefault()?.description ?? "",
                Icon = WeatherIcon.Icons[weather.FirstOrDefault()?.icon ?? ""],
                AsciiIcon = WeatherIcon.GetAsciiIcon(weather.FirstOrDefault()?.icon ?? ""),
            };
    }

    internal class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    internal class Hourly
    {
        public long dt { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public int clouds { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public Weather[] weather { get; set; }
        public float pop { get; set; }

        public HourlyWeather GetHourlyWeather() =>
            new HourlyWeather
            {
                DateTime = dt.UnixTimeStampToDateTime(),
                FeelsLike = feels_like.KalvinToCelcius(),
                ProbabilityOfPrecipitation = pop * 100,
                Temperature = temp.KalvinToCelcius(),
                Pressure = pressure,
                Humidity = humidity,
                Clouds = clouds,
                WindSpeed = wind_speed,
                WindDirection = wind_deg,
                Description = weather.FirstOrDefault()?.description ?? "",
                Icon = WeatherIcon.Icons[weather.FirstOrDefault()?.icon ?? ""],
                AsciiIcon = WeatherIcon.GetAsciiIcon(weather.FirstOrDefault()?.icon ?? ""),
            };
    }

    internal class Daily
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public Temp temp { get; set; }
        public Feels_Like feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public Weather[] weather { get; set; }
        public int clouds { get; set; }
        public float uvi { get; set; }
        public float rain { get; set; }
        public float snow { get; set; }

        public DailyWeather GetDailyWeather() =>
            new DailyWeather
            {
                DateTime = dt.UnixTimeStampToDateTime(),
                Sunrise = sunrise.UnixTimeStampToDateTime(),
                Sunset = sunset.UnixTimeStampToDateTime(),
                Rain = rain,
                Snow = snow,
                Temperature = temp.GetTemperature(),
                FeelsLike = feels_like.GetBaseTemperature(),
                Pressure = pressure,
                Humidity = humidity,
                Clouds = clouds,
                WindSpeed = wind_speed,
                WindDirection = wind_deg,
                Description = weather.FirstOrDefault()?.description ?? "",
                Icon = WeatherIcon.Icons[weather.FirstOrDefault()?.icon ?? ""],
                AsciiIcon = WeatherIcon.GetAsciiIcon(weather.FirstOrDefault()?.icon ?? ""),
            };
    }

    internal class Temp
    {
        public float day { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float night { get; set; }
        public float eve { get; set; }
        public float morn { get; set; }

        public Temperature GetTemperature() =>
            new Temperature
            {
                Min = min.KalvinToCelcius(),
                Max = max.KalvinToCelcius(),
                Day = day.KalvinToCelcius(),
                Night = night.KalvinToCelcius(),
                Eve = eve.KalvinToCelcius(),
                Morning = morn.KalvinToCelcius()
            };
    }

    internal class Feels_Like
    {
        public float day { get; set; }
        public float night { get; set; }
        public float eve { get; set; }
        public float morn { get; set; }

        public BaseTemperature GetBaseTemperature() =>
            new BaseTemperature
            {
                Day = day.KalvinToCelcius(),
                Night = night.KalvinToCelcius(),
                Eve = eve.KalvinToCelcius(),
                Morning = morn.KalvinToCelcius()
            };
    }
#pragma warning restore IDE1006 // Naming Styles
}
