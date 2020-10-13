using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataProvider.Weather;
using Guppi.Core;

namespace Alteridem.Guppi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var providers = new IDataProvider[]
            {
                new WeatherDataProvider()
            };

            var rootCommand = new RootCommand();
            foreach(var provider in providers)
            {
                rootCommand.AddCommand(provider.GetCommand());
            }

            await rootCommand.InvokeAsync(args);

            //var provider = new WeatherDataProvider();
            //if(!provider.Configured)
            //    provider.Configure();

            //await provider.Execute(false);
            //await provider.Execute(true);
        }
    }
}
