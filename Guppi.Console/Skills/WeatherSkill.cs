using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Application.Services;
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
        var view = new Command("view", "Views the weather")
        {
            new Option<bool>(new string[]{"--all", "-a" }, "Displays today's weather and the hourly forecast")
        };

        view.Handler = CommandHandler.Create(async (bool all) => await Execute(all));

        var configure = new Command("configure", "Configures the weather provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => Configure());

        return new []
        {
            new Command(Command, "Displays today's weather")
            {
               view,
               configure
            }
        };
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
                $"{dt.GetEmoji()} [silver]{dt:HH:mm}  {hour.Icon} {desc} {hour.Temperature,3}°C FeelsLike {hour.FeelsLike,3}°C {(int)(hour.ProbabilityOfPrecipitation),3}%:droplet:[/]"
            );
        }
    }

    private void DisplayShort(WeatherForecast weather)
    {
        AnsiConsoleHelper.TitleRule(":satellite_antenna: Satellite scans complete. Today's weather is...");

        int maxDesc = weather.Hourly.Select(h => h.Description).Max(d => d.Length);

        string desc = (weather.Current.Description).PadRight(maxDesc);
        AnsiConsole.MarkupLine($"[white]Current:[/][silver]  {weather.Current.Icon} {desc} {weather.Current.Temperature,3}°C FeelsLike {weather.Current.FeelsLike,3}°C[/]");

        DailyWeather today = weather.Daily.FirstOrDefault();
        desc = (today.Description).PadRight(maxDesc);
        AnsiConsole.MarkupLine($"[white]Today:[/][silver]    {today.Icon} {desc} {today.Temperature.Max,3}°C/{today.Temperature.Min,3}°C[/][silver] High/Low[/]");
    }
}
