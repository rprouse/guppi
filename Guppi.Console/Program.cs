using System;
using Guppi.Console.Actions;
using Alteridem.Guppi;
using Guppi.Application;
using Guppi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

await ConfigureServices()
    .GetRequiredService<IApplication>()
    .Run(args);

static IServiceProvider ConfigureServices() =>
    new ServiceCollection()
        .AddTransient<IApplication, Application>()
        .AddApplication()
        .AddInfrastructure()
        .AddTransient<IActionProvider, AdventOfCodeDataProvider>()
        .AddTransient<IActionProvider, CovidProvider>()
        .AddTransient<IActionProvider, CalendarDataProvider>()
        .AddTransient<IActionProvider, GitDataProvider>()
        .AddTransient<IActionProvider, HueLightsDataProvider>()
        .AddTransient<IActionProvider, NotesProvider>()
        .AddTransient<IActionProvider, StravaProvider>()
        .AddTransient<IActionProvider, WeatherDataProvider>()
        .AddTransient<IMultipleActionProvider, UtilitiesProvider>()
        .BuildServiceProvider();
