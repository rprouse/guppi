using System.Net.Http;
using System.Net.Http.Headers;
using Guppi.Domain.Interfaces;
using Guppi.Infrastructure.Services.AdventOfCode;
using Guppi.Infrastructure.Services.Calendar;
using Guppi.Infrastructure.Services.Covid;
using Guppi.Infrastructure.Services.Git;
using Guppi.Infrastructure.Services.Hue;
using Guppi.Infrastructure.Services.Notes;
using Guppi.Infrastructure.Services.Strava;
using Guppi.Infrastructure.Services.Weather;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
            services
                .AddSingleton<HttpClient>(serviceProvider => 
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
                    return client;
                })
                .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
                .AddTransient<ICalendarService, CalendarService>()
                .AddTransient<ICovidService, CovidService>()
                .AddTransient<IGitService, GitService>()
                .AddTransient<IHttpRestService, IHttpRestService>()
                .AddTransient<IHueService, HueService>()
                .AddTransient<INotesService, NotesService>()
                .AddTransient<IStravaService, StravaService>()
                .AddTransient<IWeatherService, WeatherService>();
    }
}
