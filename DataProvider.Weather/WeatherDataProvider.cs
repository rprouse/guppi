using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ColoredConsole;
using MyDay.Core;
using MyDay.Core.Extensions;

namespace DataProvider.Weather
{
    public class WeatherDataProvider : IDataProvider
    {
        WeatherConfiguration _configuration;
        HttpClient _client;

        public WeatherDataProvider()
        {
            _configuration = Configuration.Load<WeatherConfiguration>(Command);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "MyDay CLI (https://github.com/rprouse/myday)");
        }

        public string Command => "weather";

        public string Name => "Weather";

        public string Description => "Displays today's weather";

        public bool Enabled
        {
            get => _configuration.Enabled;
            set
            {
                _configuration.Enabled = value;
                _configuration.Save();
            }
        }

        public bool Configured => _configuration.Configured;

        public void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the OpenWeather API key and your location.");
        }

        public async Task Execute()
        {
            string json = await _client.GetStringAsync($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
            var weather = JsonSerializer.Deserialize<WeatherResponse>(json);

            ColorConsole.WriteLine("Today's Weather".Yellow());
            Console.WriteLine();
            ColorConsole.WriteLine($"Currently it is {weather.current.temp.KalvinToCelcius()} and feels like {weather.current.feels_like.KalvinToCelcius()}, {weather.current.weather.FirstOrDefault()?.description}".White());

            Daily today = weather.daily.FirstOrDefault();

            ColorConsole.WriteLine($"Today's high will be {today.temp.max.KalvinToCelcius()} with a low of {today.temp.min.KalvinToCelcius()}, {today.weather.FirstOrDefault()?.description}".White());
        }
    }
}
