using System;
using System.Threading.Tasks;
using ActionProvider.AdventOfCode;
using ActionProvider.Calendar;
using ActionProvider.Git;
using ActionProvider.Hue;
using ActionProvider.Notes;
using ActionProvider.Utilities;
using ActionProvider.Weather;
using Guppi.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Guppi
{
    class Program
    {
        static async Task Main(string[] args) =>
            await ConfigureServices()
                    .GetRequiredService<IApplication>()
                    .Run(args);

        static private IServiceProvider ConfigureServices() =>
            new ServiceCollection()
                .AddTransient<IApplication, Application>()
                .AddTransient<IActionProvider, AdventOfCodeDataProvider>()
                .AddTransient<IActionProvider, CalendarDataProvider>()
                .AddTransient<IActionProvider, GitDataProvider>()
                .AddTransient<IActionProvider, HueLightsDataProvider>()
                .AddTransient<IActionProvider, NotesProvider>()
                .AddTransient<IActionProvider, WeatherDataProvider>()
                .AddTransient<IMultipleActionProvider, UtilitiesProvider>()
                .BuildServiceProvider();
    }
}
