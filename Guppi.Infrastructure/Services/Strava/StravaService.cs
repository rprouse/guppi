using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Strava
{
    public class StravaService : IStravaService
    {
        const string Command = "strava";
        const string Name = "strava";

        StravaConfiguration _configuration;
        HttpClient _client;

        public StravaService()
        {
            _configuration = Configuration.Load<StravaConfiguration>(Command);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
        }

        public void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the Strava Client Id and Secret");
        }

        //public async Task<Domain.Entities.Weather.WeatherForecast> GetWeatherData()
        //{
        //    if (!Configured)
        //    {
        //        throw new UnconfiguredException("Please configure the Strava provider");
        //    }

        //    //string json = await _client.GetStringAsync($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
        //    //var weather = JsonSerializer.Deserialize<WeatherResponse>(json);
        //    //return weather.GetWeather();
        //}

        private bool Configured => _configuration.Configured;
    }
}
