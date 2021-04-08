using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Guppi.Console.Actions;
using Guppi.Domain.Common;
using Guppi.Domain.Interfaces;
using Spectre.Console;

namespace Alteridem.Guppi
{
    internal class Application : IApplication
    {
        private readonly RootCommand _rootCommand = new RootCommand();
        private readonly ISpeechService _speech;

        public Application(IEnumerable<IActionProvider> providers, IEnumerable<IMultipleActionProvider> multiProviders, ISpeechService speech)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            _rootCommand.Description = AssemblyDescription;

            var commands = providers
                .Select(p => p.GetCommand())
                .Union(multiProviders.SelectMany(m => m.GetCommands()))
                .OrderBy(c => c.Name);

            foreach (var command in commands)
                _rootCommand.AddCommand(command);
            _speech = speech;
        }

        public async Task Run(string[] args)
        {
            string saying = Sayings.Affirmative();
            _speech.Speak(saying.StripEmoji());
            AnsiConsoleHelper.TitleRule(saying.EscapeMarkup(), "gold3_1");
            AnsiConsole.WriteLine();

            await _rootCommand.InvokeAsync(args);
            await _speech.Wait();
        }

        private string AssemblyDescription =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()
                ?.Description ?? "";
    }
}
