using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spectre.Console;

namespace Guppi.Core.Services.Weather
{
    /// <summary>
    /// Used for converting OpenWeatherMap icons
    /// into their equivalent emoji
    /// </summary>
    public static class WeatherIcon
    {
        public static Dictionary<string, string> Icons { get; } = new Dictionary<string, string>
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


        public static string WindDirection(double degrees)
        {
            if (degrees < 0 || degrees > 360)
            {
                return "Unknown";
            }
            return WIND_DIRECTION[(int)Math.Round(degrees / 45) % 8];
        }

        public static readonly string[] WIND_DIRECTION = [
            "↓", "↙", "←", "↖", "↑", "↗", "→", "↘",
        ];

        public static readonly string[] MOON_PHASES = [
            "🌑", "🌒", "🌓", "🌔", "🌕", "🌖", "🌗", "🌘"
        ];

        public static string[] GetAsciiIcon(string icon)
        {
            if (AsciiIcons.TryGetValue(icon, out string[] value))
            {
                return value;
            }
            if (AsciiIcons.ContainsKey(icon[..2]))
            {
                return AsciiIcons[icon[..2]];
            }
            return AsciiIcons[""];
        }

        public static readonly ReadOnlyDictionary<string, string[]> AsciiIcons = new ( new Dictionary<string, string[]>()
        {
            { "01d", new [] {  // Day clear sky
                "[yellow]    \\   /    [/]",
                "[yellow]     .-.     [/]",
                "[yellow]  ― (   ) ―  [/]",
                "[yellow]     `-’     [/]",
                "[yellow]    /   \\    [/]" } },
            { "02d", new [] {  // Day few clouds
                "[yellow]   \\  /[/]      ",
                "[yellow] _ /\"\"[/][grey62].-.    [/]",
                "[yellow]   \\_[/][grey62](   ).  [/]",
                "[yellow]   /[/][grey62](___(__) [/]",
                "             " } },
            { "03", new [] {   // Day/Night scattered clouds
                "             ",
                "[grey62]     .--.    [/]",
                "[grey62]  .-(    ).  [/]",
                "[grey62] (___.__)__) [/]",
                "             " } },
            { "04", new [] {  // Day/Night cloudy
                "             ",
                "[grey42]     .--.    [/]",
                "[grey42]  .-(    ).  [/]",
                "[grey42] (___.__)__) [/]",
                "             " } },
            { "09", new [] { // Day/Night shower rain
                "[yellow] _`/\"\"[/][grey62].-.    [/]",
                "[yellow]  ,\\_[/][grey62](   ).  [/]",
                "[yellow]   /[/][grey62](___(__) [/]",
                "[skyblue2]     ‘ ‘ ‘ ‘ [/]",
                "[skyblue2]    ‘ ‘ ‘ ‘  [/]" } },
            { "10", new [] {  // Day/Night rain
                "[grey42]      .-.    [/]",
                "[grey42]     (   ).  [/]",
                "[grey42]    (___(__) [/]",
                "[dodgerblue2]   ‚‘‚‘‚‘‚‘  [/]",
                "[dodgerblue2]   ‚’‚’‚’‚’  [/]" } },
            { "11", new [] {  // Day/Night thunderstorm
                "[grey42]     .-.     [/]",
                "[grey42]    (   ).   [/]",
                "[grey42]   (___(__)  [/]",
                "[dodgerblue2]  ‚‘[/][khaki1]⚡[/][dodgerblue2]‘‚[/][khaki1]⚡[/][dodgerblue2]‚‘ [/]",
                "[dodgerblue2]  ‚’‚’[/][khaki1]⚡[/][dodgerblue2]’‚’  [/]" } },
            { "13", new [] {  // Day/Night snow
                "[grey42]      .-.    [/]",
                "[grey42]     (   ).  [/]",
                "[grey42]    (___(__) [/]",
                "[grey89]    * * * *  [/]",
                "[grey89]   * * * *   [/]" } },
            { "50", new [] {   // Day/Night mist
                "             ",
                "[grey78] _ - _ - _ - [/]",
                "[grey78]  _ - _ - _  [/]",
                "[grey78] _ - _ - _ - [/]",
                "             " } },
            { "01n", new [] {  // Night clear sky
                "[lightgoldenrod1]    _.-.     [/]",
                "[lightgoldenrod1]   /o  o \\   [/]",
                "[lightgoldenrod1]  |   O   |  [/]",
                "[lightgoldenrod1]   \\ o O /   [/]",
                "[lightgoldenrod1]    `-_-’    [/]" } },
            { "02n", new [] {  // Night few clouds
                "[lightgoldenrod1]    _.-.     [/]",
                "[lightgoldenrod1]   /o [/][grey62].-.    [/]",
                "[lightgoldenrod1]  |  [/][grey62](   ).  [/]",
                "[lightgoldenrod1]   \\[/][grey62](___(__) [/]",
                "[lightgoldenrod1]    `-_-’    [/]" } },
            { "", new [] {  // Unknown
                "    .-.      ",
                "     __)     ",
                "    (        ",
                "     `-’     ",
                "      •      " } },
        });
    }
}
