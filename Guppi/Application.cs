using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core;
using Spectre.Console;

namespace Alteridem.Guppi
{
    internal class Application : IApplication
    {
        private readonly RootCommand _rootCommand = new RootCommand();

        public Application(IEnumerable<IDataProvider> providers, IEnumerable<IMultipleDataProvider> multiProviders)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var commands = providers
                .Select(p => p.GetCommand())
                .Union(multiProviders.SelectMany(m => m.GetCommands()))
                .OrderBy(c => c.Name);

            foreach (var command in commands)
                _rootCommand.AddCommand(command);
        }

        public async Task Run(string[] args)
        {
            AnsiConsole.MarkupLine("[cyan2 bold]{0}[/]", Sayings.Affirmative().EscapeMarkup());
            AnsiConsole.WriteLine();

            await _rootCommand.InvokeAsync(args);
        }
    }
}
