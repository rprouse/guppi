using System.CommandLine;
using System.Threading.Tasks;
using ColoredConsole;
using DataProvider.AdventOfCode;
using DataProvider.Calendar;
using DataProvider.Git;
using DataProvider.Hue;
using DataProvider.Notes;
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
                new GitDataProvider(),
                new HueLightsDataProvider(),
                new NotesProvider(),
                new WeatherDataProvider(),
            };

            var rootCommand = new RootCommand();
            foreach(var provider in providers)
            {
                rootCommand.AddCommand(provider.GetCommand());
            }

            ColorConsole.WriteLine(Sayings.Affirmative().Cyan());
            ColorConsole.WriteLine();

            await rootCommand.InvokeAsync(args);
        }
    }
}
