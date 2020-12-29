using System;
using System.Threading.Tasks;
using DataProvider.AdventOfCode;
using DataProvider.Calendar;
using DataProvider.Git;
using DataProvider.Hue;
using DataProvider.Notes;
using DataProvider.Utilities;
using DataProvider.Weather;
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
                .AddTransient<IDataProvider, AdventOfCodeDataProvider>()
                .AddTransient<IDataProvider, CalendarDataProvider>()
                .AddTransient<IDataProvider, GitDataProvider>()
                .AddTransient<IDataProvider, HueLightsDataProvider>()
                .AddTransient<IDataProvider, NotesProvider>()
                .AddTransient<IDataProvider, WeatherDataProvider>()
                .AddTransient<IMultipleDataProvider, UtilitiesProvider>()
                .BuildServiceProvider();
    }
}
