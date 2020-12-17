using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using ColoredConsole;
using DataProvider.AdventOfCode;
using DataProvider.Calendar;
using DataProvider.Git;
using DataProvider.Hue;
using DataProvider.Notes;
using DataProvider.Utilities;
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
                new AdventOfCodeDataProvider(),
                new CalendarDataProvider(),
                new GitDataProvider(),
                new HueLightsDataProvider(),
                new NotesProvider(),
                new WeatherDataProvider(),
            };

            var multiProviders = new IMultipleDataProvider[]
            {
                new UtilitiesProvider(),
            };

            var multicommands = multiProviders.SelectMany(m => m.GetCommands());
            var commands = providers.Select(p => p.GetCommand()).Union(multicommands).OrderBy(c => c.Name);

            var rootCommand = new RootCommand();
            foreach(var command in commands)
            {
                rootCommand.AddCommand(command);
            }

            ColorConsole.WriteLine(Sayings.Affirmative().Cyan());
            ColorConsole.WriteLine();

            await rootCommand.InvokeAsync(args);
        }
    }
}
