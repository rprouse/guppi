using Guppi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services
        .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
        .AddTransient<IAsciiService, AsciiService>()
        .AddTransient<ICalendarService, CalendarService>()
        .AddTransient<IDictionaryService, DictionaryService>()
        .AddTransient<IGitService, GitService>()
        .AddTransient<IHueLightService, HueLightService>()
        .AddTransient<INoteService, NoteService>()
        .AddTransient<IStravaService, StravaService>()
        .AddTransient<IUtilitiesService, UtilitiesService>()
        .AddTransient<IWeatherService, WeatherService>();
}
