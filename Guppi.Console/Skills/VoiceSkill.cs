using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces.Providers;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    public class VoiceSkill(ISpeechProvider speech) : ISkill
    {
        private readonly ISpeechProvider _speech = speech;

        public IEnumerable<Command> GetCommands()
        {
            if (!OperatingSystem.IsWindows())
                return Enumerable.Empty<Command>();

            var list = new Command("list", "List and set installed voices");
            list.AddAlias("ls");
            list.Handler = CommandHandler.Create(List);

            var voice = new Command("voice", "Work with voice on Windows")
            {
                list
            };

            return new[] { voice };
        }

        private void List()
        {
            var voices = _speech.ListVoices().ToArray();

            AnsiConsoleHelper.TitleRule(":loudspeaker: Installed voices");

            for (int i = 0; i < voices.Length; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}: {voices[i].Description}");
            }
            AnsiConsole.WriteLine();

            int selected = AnsiConsole.Prompt(
                new TextPrompt<int>("Desired voice?")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]That's not a valid voice[/]")
                    .Validate(i =>
                    {
                        if (i < 1 || i > voices.Length)
                            return ValidationResult.Error();
                        return ValidationResult.Success();
                    }));

            _speech.SetVoice(voices[selected - 1]);
        }
    }
}
