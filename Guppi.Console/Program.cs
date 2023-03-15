using System;
using Guppi.Console.Skills;
using Guppi.Application;
using Guppi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Guppi.Console;

await ConfigureServices()
    .GetRequiredService<IApplication>()
    .Run(args);

static IServiceProvider ConfigureServices() =>
    new ServiceCollection()
        .AddTransient<IApplication, Application>()
        .AddTransient<ISkill, AdventOfCodeSkill>()
        .AddTransient<ISkill, AsciiSkill>()
        .AddTransient<ISkill, CalendarSkill>()
        .AddTransient<ISkill, GitSkill>()
        .AddTransient<ISkill, HueLightsSkill>()
        .AddTransient<ISkill, NotesSkill>()
        .AddTransient<ISkill, StravaSkill>()
        .AddTransient<ISkill, WeatherSkill>()
        .AddTransient<ISkill, UtilitiesSkill>()
        .AddApplication()
        .AddInfrastructure()
        .BuildServiceProvider();
