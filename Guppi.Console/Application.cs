using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
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
        private readonly Parser _parser;
        private readonly ISpeechService _speech;

        public Application(IEnumerable<IActionProvider> providers, IEnumerable<IMultipleActionProvider> multiProviders, ISpeechService speech)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            _speech = speech;

            var rootCommand = new RootCommand(AssemblyDescription)
            {
                new Option<bool>(new [] { "--silent", "-s" }, () => false, "Don't display or speak initial quip.")
            };

            var commands = providers
                .Select(p => p.GetCommand())
                .Union(multiProviders.SelectMany(m => m.GetCommands()))
                .OrderBy(c => c.Name);

            foreach (var command in commands)
                rootCommand.AddCommand(command);

            var commandLineBuilder = new CommandLineBuilder(rootCommand);
            commandLineBuilder.UseMiddleware(async (context, next) =>
            {
                var silent = context.ParseResult
                    .RootCommandResult
                    .OptionResult("--silent")
                    ?.GetValueOrDefault<bool>();
                if (silent != true)
                {
                    Quip();
                }
                await next(context);
            });
            commandLineBuilder.UseDefaults();
            _parser = commandLineBuilder.Build();
        }

        public async Task Run(string[] args)
        {
            await _parser.InvokeAsync(args);
            await _speech.Wait();
        }

        private void Quip()
        {
            string saying = Sayings.Affirmative();
            _speech.Speak(saying.StripEmoji());
            AnsiConsoleHelper.TitleRule(saying.EscapeMarkup(), "gold3_1");
            AnsiConsole.WriteLine();
        }

        private string AssemblyDescription =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()
                ?.Description ?? "";
    }
}
