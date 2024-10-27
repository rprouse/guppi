using System;
using Guppi.Console.Skills;
using Guppi.Core;
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
        .AddTransient<ISkill, BillsSkill>()
        .AddTransient<ISkill, CalendarSkill>()
        .AddTransient<ISkill, DictionarySkill>()
        .AddTransient<ISkill, GitSkill>()
        .AddTransient<ISkill, HueLightsSkill>()
        .AddTransient<ISkill, IpSkill>()
        .AddTransient<ISkill, ManifestoSkill>()
        .AddTransient<ISkill, NotesSkill>()
        .AddTransient<ISkill, OpenAISkill>()
        .AddTransient<ISkill, RC2014Skill>()
        .AddTransient<ISkill, SerialPortSkill>()
        .AddTransient<ISkill, StravaSkill>()
        .AddTransient<ISkill, TodoSkill>()
        .AddTransient<ISkill, WeatherSkill>()
        .AddTransient<ISkill, UtilitiesSkill>()
        .AddTransient<ISkill, VoiceSkill>()
        .AddCore()
        .BuildServiceProvider();
