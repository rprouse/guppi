using System.Collections.Generic;
using Spectre.Console;

namespace Guppi.Application.Queries.Weather
{
    /// <summary>
    /// Used for converting OpenWeatherMap icons
    /// into their equivalent emoji
    /// </summary>
    internal static class WeatherIcon
    {
        internal static Dictionary<string, string> Icons { get; } = new Dictionary<string, string>
        {
            { "01d", Emoji.Known.Sun },  // Day clear sky
            { "02d", Emoji.Known.SunBehindSmallCloud },  // Day few clouds
            { "03d", Emoji.Known.SunBehindLargeCloud },  // Day scattered clouds
            { "04d", Emoji.Known.Cloud },  // Day broken clouds
            { "09d", Emoji.Known.SunBehindRainCloud },  // Day shower rain
            { "10d", Emoji.Known.CloudWithRain },  // Day rain
            { "11d", Emoji.Known.CloudWithLightningAndRain },  // Day thunderstorm
            { "13d", Emoji.Known.Snowflake },  // Day snow
            { "50d", Emoji.Known.Fog },  // Day mist
            { "01n", Emoji.Known.EightPointedStar },  // Night clear sky
            { "02n", Emoji.Known.SunBehindSmallCloud },  // Night few clouds
            { "03n", Emoji.Known.SunBehindLargeCloud },  // Night scattered clouds
            { "04n", Emoji.Known.Cloud },  // Night broken clouds
            { "09n", Emoji.Known.SunBehindRainCloud },  // Night shower rain
            { "10n", Emoji.Known.CloudWithRain },  // Night rain
            { "11n", Emoji.Known.CloudWithLightningAndRain },  // Night thunderstorm
            { "13n", Emoji.Known.Snowflake },  // Night snow
            { "50n", Emoji.Known.Fog },  // Night mist
            { "", "" }  // Unknown
        };
    }
}
