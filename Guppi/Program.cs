using System;
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
        .BuildServiceProvider();
