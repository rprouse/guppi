using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Services;
using Guppi.Core.Services.Weather;
using Guppi.Domain.Entities.Weather;
using Spectre.Console;
using Location = Guppi.Domain.Entities.Weather.Location;

namespace Guppi.Console.Skills;

internal class WeatherSkill(IWeatherService service) : ISkill
{
    const string Command = "weather";
    private readonly IWeatherService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var location = new Option<string>(new string[] { "--location", "-l" }, () => null, "Location to view weather for");

        var view = new Command("view", "Views the current weather") { location };
        view.Handler = CommandHandler.Create(async (string location) => await Execute(location, false));
        view.AddAlias("now");

        var hourly = new Command("hourly", "Views the hourly weather") { location };
        hourly.Handler = CommandHandler.Create(async (string location) => await Execute(location, true));

        var daily = new Command("daily", "Views the daily weather") { location };
        daily.Handler = CommandHandler.Create(async (string location) => await Daily(location));

        var configure = new Command("configure", "Configures the weather provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => Configure());

        return new[]
        {
            new Command(Command, "Displays today's weather")
            {
               view,
               hourly,
               daily,
               configure
            }
        };
    }

    private async Task<WeatherForecast> GetWeather(string location)
    {
        // Default Location
        if (string.IsNullOrWhiteSpace(location))
            return await _service.GetWeather();

        var locations = await _service.GetLocations(location);

        if (!locations.Any())
            throw new UnconfiguredException("Location not found");

        Location selected;
        if (locations.Count() == 1)
        {
            selected = locations.First();
        }
        else
        {
            selected = AnsiConsole.Prompt(
                new SelectionPrompt<Location>()
                    .Title("Select a location")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to see more locations)[/]")
                    .AddChoices(locations));
        }

        AnsiConsole.MarkupLine($":round_pushpin: [cyan]{selected}[/]");
        AnsiConsole.WriteLine();

        return await _service.GetWeather(selected.Latitude.ToString(), selected.Longitude.ToString());
    }

    private async Task Daily(string location)
    {
        try
        {
            WeatherForecast weather = await GetWeather(location);

            DisplayShort(weather);

            AnsiConsole.WriteLine();

            AnsiConsoleHelper.TitleRule(":radio: Communications & External Sensors: Ready/Standby. Future weather is...");

            DailyWeatherTable(weather.Daily);
            DailyWeatherTable(weather.Daily.Skip(4).ToArray());
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
    }

    private static void DailyWeatherTable(DailyWeather[] daily)
    {
        var table = new Table();

        for (int i = 0; i < 4 && i < daily.Length; i++)
            table.AddColumn(new TableColumn($"[silver]{daily[i].DateTime:ddd MMM dd}[/]"));

        table.AddRow(WeatherLineOne(daily));
        table.AddRow(WeatherLineTwo(daily));
        table.AddRow(WeatherLineThree(daily));
        table.AddRow(WeatherLineFour(daily));
        table.AddRow(WeatherLineFive(daily));

        AnsiConsole.Write(table);
    }

    private static IEnumerable<Markup> WeatherLineOne(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[0]} [silver]{daily[i].Description}[/]");
    }

    private static IEnumerable<Markup> WeatherLineTwo(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[1]} [silver]↑[/][{daily[i].Temperature.Max.ToColor()}]{daily[i].Temperature.Max}[/][silver]°C ↓[/][{daily[i].Temperature.Min.ToColor()}]{daily[i].Temperature.Min}[/][silver]°C[/]");

    }

    private static IEnumerable<Markup> WeatherLineThree(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[2]} [silver]{WeatherIcon.WindDirection(daily[i].WindDirection)} [/][gold3_1]{daily[i].WindSpeed:F0}[/][silver] km/h[/]");
    }

    private static IEnumerable<Markup> WeatherLineFour(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[3]} [skyblue2]:droplet:{daily[i].Rain}mm[/] [grey89]:snowflake:{daily[i].Snow}mm[/]");
    }

    private static IEnumerable<Markup> WeatherLineFive(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[4]} [silver]{daily[i].Pressure}mb {daily[i].Humidity,3}%[/]");
    }

    private async Task Execute(string location, bool all)
    {
        try
        {
            WeatherForecast weather = await GetWeather(location);

            if (all)
                DisplayLong(weather);
            else
                DisplayShort(weather);
            AnsiConsoleHelper.Rule("white");
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
    }

    private void Configure() => _service.Configure();

    private static void DisplayLong(WeatherForecast weather)
    {
        DisplayShort(weather);

        DateTime last = DateTime.MinValue;
        int maxDesc = weather.Hourly.Select(h => h.Description).Max(d => d.Length);
        foreach (var hour in weather.Hourly)
        {
            DateTime dt = hour.DateTime;
            if (dt.Date != last.Date)
            {
                AnsiConsoleHelper.Rule("white");
                AnsiConsole.MarkupLine($":calendar: [white bold]{dt:ddd MMM dd}[/]");
                AnsiConsoleHelper.Rule("silver");
            }
            last = dt;
            string desc = (hour.Description).PadRight(maxDesc);
            AnsiConsole.MarkupLine(
                $"{dt.GetEmoji()} [silver]{dt:HH:mm}  {hour.Icon} {desc} [{hour.Temperature.ToColor()}]{hour.Temperature,3}[/]°C FeelsLike [{hour.FeelsLike.ToColor()}]{hour.FeelsLike,3}[/]°C {(int)(hour.ProbabilityOfPrecipitation),3}%:droplet:[/]"
            );
        }
    }

    private static void DisplayShort(WeatherForecast weather)
    {
        AnsiConsoleHelper.TitleRule(":satellite_antenna: Satellite scans complete. Today's current weather is...");

        AnsiConsole.Markup(weather.Current.AsciiIcon[0]);
        AnsiConsole.MarkupLine($" [silver]{weather.Current.Description}[/]");
        AnsiConsole.Markup(weather.Current.AsciiIcon[1]);
        AnsiConsole.MarkupLine($" [{weather.Current.Temperature.ToColor()}]{weather.Current.Temperature}[/][silver]([/][{weather.Current.FeelsLike.ToColor()}]{weather.Current.FeelsLike}[/][silver]) °C[/]");
        AnsiConsole.Markup(weather.Current.AsciiIcon[2]);
        AnsiConsole.MarkupLine($" [silver]{WeatherIcon.WindDirection(weather.Current.WindDirection)} [/][gold3_1]{weather.Current.WindSpeed:F0}-{weather.Current.WindGust:F0}[/][silver] km/h[/]");
        AnsiConsole.Markup(weather.Current.AsciiIcon[3]);
        AnsiConsole.MarkupLine($" [silver]{weather.Current.Pressure} mb[/]");

        DailyWeather today = weather.Daily.FirstOrDefault();
        if (today is null) return;

        AnsiConsole.Markup(weather.Current.AsciiIcon[4]);
        AnsiConsole.MarkupLine($" [silver]↑[{today.Temperature.Max.ToColor()}]{today.Temperature.Max}[/]°C ↓[{today.Temperature.Min.ToColor()}]{today.Temperature.Min}[/]°C[/]");
    }
}
