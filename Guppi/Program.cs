using System;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Infrastructure;
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
                .AddApplication()
                .AddInfrastructure()
                .BuildServiceProvider();
    }
}
