using System.IO;
using System;
using Guppi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Setup the Todo application's services
        string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo.json");

        Alteridem.Todo.Application.DependencyInjection.AddApplication(services);
        Alteridem.Todo.Infrastructure.DependencyInjection.AddInfrastructure(services, configFile);

        return services
            .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
            .AddTransient<IAsciiService, AsciiService>()
            .AddTransient<ICalendarService, CalendarService>()
            .AddTransient<IDictionaryService, DictionaryService>()
            .AddTransient<IGitService, GitService>()
            .AddTransient<IHueLightService, HueLightService>()
            .AddTransient<INoteService, NoteService>()
            .AddTransient<IOpenAIService, OpenAIService>()
            .AddTransient<ISerialPortService, SerialPortService>()
            .AddTransient<IStravaService, StravaService>()
            .AddSingleton<ITodoService, TodoService>()
            .AddTransient<IUtilitiesService, UtilitiesService>()
            .AddTransient<IWeatherService, WeatherService>();
    }
}
