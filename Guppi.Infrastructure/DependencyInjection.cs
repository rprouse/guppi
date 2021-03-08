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
                .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
                .AddTransient<ICalendarService, CalendarService>()
                .AddTransient<ICovidService, CovidService>()
                .AddTransient<IGitService, GitService>()
                .AddTransient<IHueService, HueService>()
                .AddTransient<INotesService, NotesService>()
                .AddTransient<IStravaService, StravaService>()
                .AddTransient<IWeatherService, WeatherService>();
    }
}
