using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Guppi.Core.Extensions;
using Guppi.Console.Skills;
using Guppi.Core.Common;
using Guppi.Core.Interfaces;
using Spectre.Console;

namespace Guppi.Console
{
    internal class Application : IApplication
    {
        private readonly Parser _parser;
        private readonly ISpeechService _speech;

        public Application(IEnumerable<ISkill> skills, ISpeechService speech)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            _speech = speech;

            var silentOption = new Option<bool>(new[] { "--silent", "-s" }, () => false, "Don't display or speak initial quip.");
            var rootCommand = new RootCommand(AssemblyDescription) { silentOption };

            var commands = skills
                .SelectMany(m => m.GetCommands())
                .OrderBy(c => c.Name);

            foreach (var command in commands)
                rootCommand.AddCommand(command);

            _parser = new CommandLineBuilder(rootCommand)
                .AddMiddleware(async (context, next) =>
                {
                    var silent = context.ParseResult
                        .RootCommandResult
                        .FindResultFor(silentOption)
                        ?.GetValueOrDefault<bool>();
                    if (silent != true)
                    {
                        Quip();
                    }
                    await next(context);
                })
                .RegisterWithDotnetSuggest()
                .UseDefaults()
                .Build();
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

        private static string AssemblyDescription =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()
                ?.Description ?? "";
    }
}
