using System;

namespace Guppi.Domain.Entities.Weather
{
    public class WeatherForecast
    {
        public CurrentWeather Current { get; set; }
        public HourlyWeather[] Hourly { get; set; }
        public DailyWeather[] Daily { get; set; }
    }

    public class CurrentWeather : WeatherData
    {
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public float WindGust { get; set; }
        public int Temperature { get; set; }
        public int FeelsLike { get; set; }
    }

    public class DailyWeather : WeatherData
    {
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public float Rain { get; set; }
        public float Snow { get; set; }

        public Temperature Temperature { get; set; }

        public BaseTemperature FeelsLike { get; set; }
    }

    public class HourlyWeather : WeatherData
    {
        public float Temperature { get; set; }
        public float FeelsLike { get; set; }
        public float ProbabilityOfPrecipitation { get; set; }
    }

    public class WeatherData
    {
        public DateTime DateTime { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int Clouds { get; set; }
        public float WindSpeed { get; set; }
        public int WindDirection { get; set; }

        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Temperature : BaseTemperature
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class BaseTemperature
    {
        public int Day { get; set; }
        public int Night { get; set; }
        public int Eve { get; set; }
        public int Morning { get; set; }
    }
}
