using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Application.Services;
using Guppi.Application.Services.Weather;
using Guppi.Domain.Entities.Weather;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class WeatherSkill : ISkill
{
    const string Command = "weather";
    private readonly IWeatherService _service;

    public WeatherSkill(IWeatherService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var view = new Command("view", "Views the current weather");
        view.Handler = CommandHandler.Create(async () => await Execute(false));

        var hourly = new Command("hourly", "Views the hourly weather");
        hourly.Handler = CommandHandler.Create(async () => await Execute(true));

        var daily = new Command("daily", "Views the daily weather");
        daily.Handler = CommandHandler.Create(async () => await Daily());

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

    private async Task Daily()
    {
        try
        {
            WeatherForecast weather = await _service.GetWeather();

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

    private void DailyWeatherTable(DailyWeather[] daily)
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

    private IEnumerable<Markup> WeatherLineOne(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[0]} [silver]{daily[i].Description}[/]");
    }

    private IEnumerable<Markup> WeatherLineTwo(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[1]} [silver]↑[/][{daily[i].Temperature.Max.ToColor()}]{daily[i].Temperature.Max}[/][silver]°C ↓[/][{daily[i].Temperature.Min.ToColor()}]{daily[i].Temperature.Min}[/][silver]°C[/]");

    }

    private IEnumerable<Markup> WeatherLineThree(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[2]} [silver]{WeatherIcon.WindDirection(daily[i].WindDirection)} [/][gold3_1]{daily[i].WindSpeed:F0}[/][silver] km/h[/]");
    }

    private IEnumerable<Markup> WeatherLineFour(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[3]} [skyblue2]:droplet:{daily[i].Rain}mm[/] [grey89]:snowflake:{daily[i].Snow}mm[/]");
    }

    private IEnumerable<Markup> WeatherLineFive(DailyWeather[] daily)
    {
        for (int i = 0; i < 4 && i < daily.Length; i++)
            yield return new Markup($"{daily[i].AsciiIcon[4]} [silver]{daily[i].Pressure}mb {daily[i].Humidity,3}%[/]");
    }

    private async Task Execute(bool all)
    {
        try
        {
            WeatherForecast weather = await _service.GetWeather();

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

    private void DisplayLong(WeatherForecast weather)
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

    private void DisplayShort(WeatherForecast weather)
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

        AnsiConsole.Markup(weather.Current.AsciiIcon[4]);
        AnsiConsole.MarkupLine($" [silver]↑[{today.Temperature.Max.ToColor()}]{today.Temperature.Max}[/]°C ↓[{today.Temperature.Min.ToColor()}]{today.Temperature.Min}[/]°C[/]");
    }
}
