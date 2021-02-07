using System;
using ActionProvider.AdventOfCode;
using ActionProvider.Calendar;
using ActionProvider.Git;
using ActionProvider.Hue;
using ActionProvider.Notes;
using ActionProvider.Utilities;
using ActionProvider.Weather;
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
        .AddTransient<IActionProvider, CalendarDataProvider>()
        .AddTransient<IActionProvider, GitDataProvider>()
        .AddTransient<IActionProvider, HueLightsDataProvider>()
        .AddTransient<IActionProvider, NotesProvider>()
        .AddTransient<IActionProvider, WeatherDataProvider>()
        .AddTransient<IMultipleActionProvider, UtilitiesProvider>()
        .BuildServiceProvider();
