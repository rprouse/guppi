using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ColoredConsole;
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
                ColorConsole.WriteLine("Please configure the weather provider".Yellow());
                return;
            }

            WeatherResponse weather = await GetWeatherData();
            if (all)
                DisplayLong(weather);
            else
                DisplayShort(weather);
        }

        private bool Configured => _configuration.Configured;

        private void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the OpenWeather API key and your location.");
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
            foreach (var hour in weather.hourly)
            {
                DateTime dt = hour.dt.UnixTimeStampToDateTime();
                if (dt.Date != last.Date)
                    Console.WriteLine();
                last = dt;
                ColorConsole.WriteLine($"{dt:ddd HH:mm}: ".Cyan(), hour.temp.KalvinToCelcius().Green(), " feels like ".White(), hour.feels_like.KalvinToCelcius().Green(), $", {hour.weather.FirstOrDefault()?.description}".White());
            }
        }

        private void DisplayShort(WeatherResponse weather)
        {
            ColorConsole.WriteLine("Today's Weather".Yellow());
            Console.WriteLine();

            ColorConsole.WriteLine("Current: ".Cyan(), weather.current.temp.KalvinToCelcius().Green(), " feels like ".White(), weather.current.feels_like.KalvinToCelcius().Green(), $", {weather.current.weather.FirstOrDefault()?.description}".White());

            Daily today = weather.daily.FirstOrDefault();

            ColorConsole.WriteLine("Today:   ".Cyan(), "High of ".White(), today.temp.max.KalvinToCelcius().Green(), ", low of ".White(), today.temp.min.KalvinToCelcius().Green(), $", {today.weather.FirstOrDefault()?.description}".White());
        }
    }
}
