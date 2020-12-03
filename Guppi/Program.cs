using System.CommandLine;
using System.Threading.Tasks;
using DataProvider.AdventOfCode;
using DataProvider.Calendar;
using DataProvider.Hue;
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
                // Keep these in alpha order of the commands
                new AdventOfCodeDataProvider(),
                new CalendarDataProvider(),
                new HueLightsDataProvider(),
                new WeatherDataProvider(),
            };

            var rootCommand = new RootCommand();
            foreach(var provider in providers)
            {
                rootCommand.AddCommand(provider.GetCommand());
            }

            await rootCommand.InvokeAsync(args);
        }
    }
}
