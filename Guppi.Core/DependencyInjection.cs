using System.IO;
using System;
using Guppi.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Guppi.Core.Providers.Calendar;
using Guppi.Core.Providers.Git;
using Guppi.Core.Providers.Hue;
using Guppi.Core.Providers;
using System.Net.Http.Headers;
using System.Net.Http;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Interfaces.Providers;

namespace Guppi.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        // Setup the Todo application's services
        string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo.json");

        Alteridem.Todo.Application.DependencyInjection.AddApplication(services);
        Alteridem.Todo.Infrastructure.DependencyInjection.AddInfrastructure(services, configFile);

        return services
            // Add the providers
            .AddSingleton<HttpClient>(serviceProvider =>
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
                return client;
            })
            .AddTransient<ICalendarProvider, GoogleCalendarProvider>()
            .AddTransient<ICalendarProvider, ICalCalendarProvider>()
            .AddTransient<IGitProvider, GitProvider>()
            .AddTransient<IHttpRestProvider, HttpRestProvider>()
            .AddTransient<IHueProvider, HueProvider>()
            .AddTransient<IProcessProvider, ProcessProvider>()
            .AddSingleton<ISpeechProvider, SpeechProvider>()

            // Add the services
            .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
            .AddTransient<IAsciiService, AsciiService>()
            .AddTransient<IBillService, BillService>()
            .AddTransient<ICalendarService, CalendarService>()
            .AddTransient<IDictionaryService, DictionaryService>()
            .AddTransient<IGitService, GitService>()
            .AddTransient<IHueLightService, HueLightService>()
            .AddTransient<IIPService, IPService>()
            .AddTransient<INoteService, NoteService>()
            .AddTransient<IOpenAIService, OpenAIService>()
            .AddTransient<ISerialPortService, SerialPortService>()
            .AddTransient<IStravaService, StravaService>()
            .AddSingleton<ITodoService, TodoService>()
            .AddTransient<IUtilitiesService, UtilitiesService>()
            .AddTransient<IWeatherService, WeatherService>();
    }
}
