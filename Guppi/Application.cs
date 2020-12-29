using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using ColoredConsole;
using Guppi.Core;

namespace Alteridem.Guppi
{
    internal class Application : IApplication
    {
        private readonly RootCommand _rootCommand = new RootCommand();

        public Application(IEnumerable<IDataProvider> providers, IEnumerable<IMultipleDataProvider> multiProviders)
        {
            var commands = providers
                .Select(p => p.GetCommand())
                .Union(multiProviders.SelectMany(m => m.GetCommands()))
                .OrderBy(c => c.Name);

            foreach (var command in commands)
                _rootCommand.AddCommand(command);
        }

        public async Task Run(string[] args)
        {
            ColorConsole.WriteLine(Sayings.Affirmative().Cyan());
            ColorConsole.WriteLine();

            await _rootCommand.InvokeAsync(args);
        }
    }
}
