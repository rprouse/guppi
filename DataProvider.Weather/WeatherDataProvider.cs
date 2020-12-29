using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;
using Guppi.Core;
using Guppi.Core.Extensions;

namespace DataProvider.Weather
{
    public class WeatherDataProvider : IDataProvider
    {
        const string Command = "weather";
        const string Name = "Weather";

        WeatherConfiguration _configuration;
        HttpClient _client;

        public WeatherDataProvider()
        {
            _configuration = Configuration.Load<WeatherConfiguration>(Command);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/myday)");
        }

        public Command GetCommand()
        {
            var view = new Command("view", "Views the weather")
            {
                new Option<bool>(new string[]{"--all", "-a" }, "Displays today's weather and the hourly forecast")
            };

            view.Handler = CommandHandler.Create(async (bool all) => await Execute(all));

            var configure = new Command("configure", "Configures the weather provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(() => Configure());

            return new Command(Command, "Displays today's weather")
            {
               view,
               configure
            };
        }

        private async Task Execute(bool all)
        {
            if (!Configured)
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: Please configure the weather provider]][/]");
                return;
            }

            WeatherResponse weather = await GetWeatherData();
            if (all)
                DisplayLong(weather);
            else
                DisplayShort(weather);
            AnsiConsoleHelper.Rule("white");
        }

        private bool Configured => _configuration.Configured;

        private void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the OpenWeather API key and your location");
        }

        private async Task<WeatherResponse> GetWeatherData()
        {
            string json = await _client.GetStringAsync($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
            var weather = JsonSerializer.Deserialize<WeatherResponse>(json);
            return weather;
        }

        private void DisplayLong(WeatherResponse weather)
        {
            DisplayShort(weather);

            DateTime last = DateTime.MinValue;
            int maxDesc = weather.hourly.Select(h => h.weather.FirstOrDefault()?.description ?? "").Max(d => d.Length);
            foreach (var hour in weather.hourly)
            {
                DateTime dt = hour.dt.UnixTimeStampToDateTime();
                if (dt.Date != last.Date)
                {
                    AnsiConsoleHelper.Rule("white");
                    AnsiConsole.MarkupLine($"[white bold]{dt:ddd MMM dd}[/]");
                    AnsiConsoleHelper.Rule("silver");
                }
                last = dt;
                string icon = hour.weather.FirstOrDefault()?.icon ?? "";
                string desc = (hour.weather.FirstOrDefault()?.description ?? "").PadRight(maxDesc);
                AnsiConsole.MarkupLine(
                    $"[silver]{dt:HH:mm}  {WeatherIcon.Icons[icon]} {desc} {hour.temp.KalvinToCelcius(),5} FeelsLike {hour.feels_like.KalvinToCelcius(),5} {(int)(hour.pop*100),3}%:droplet:[/]"
                );
            }
        }

        private void DisplayShort(WeatherResponse weather)
        {
            AnsiConsoleHelper.TitleRule("Satellite scans complete. Today's weather is...");

            AnsiConsole.MarkupLine($"[white]Current:  {weather.current.temp.KalvinToCelcius()}[/][silver], FeelsLike[/] [white]{weather.current.feels_like.KalvinToCelcius()}[/] [silver]{weather.current.weather.FirstOrDefault()?.description}[/]");

            Daily today = weather.daily.FirstOrDefault();

            AnsiConsole.MarkupLine($"[white]Today:    [/][silver]High/Low[/] [white]{today.temp.max.KalvinToCelcius()}/{today.temp.min.KalvinToCelcius()}[/] [silver]{today.weather.FirstOrDefault()?.description}[/]");
        }
    }
}
