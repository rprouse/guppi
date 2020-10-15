using System.CommandLine;
using System.Threading.Tasks;
using DataProvider.Calendar;
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
                new WeatherDataProvider(),
                new CalendarDataProvider()
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
