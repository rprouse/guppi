using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class DictionarySkill : ISkill
{
    private readonly IDictionaryService _service;

    public DictionarySkill(IDictionaryService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var dict = new Command("dictionary", "Lookup a word in the Dictionary")
        {
            new Argument<string>("word", "The word to lookup. Use 'config' to configure")
        };
        dict.AddAlias("dict");
        dict.Handler = CommandHandler.Create<string>(LookupDictionaryFor);

        var thes = new Command("thesaurus", "Lookup a word in the Thesaurus")
        {
            new Argument<string>("word", "The word to lookup. Use 'config' to configure")
        };
        thes.AddAlias("thes");
        thes.Handler = CommandHandler.Create<string>(LookupThesaurusFor);

        return new[] { dict, thes };
    }

    private async Task LookupDictionaryFor(string word)
    {
        if (word.ToLowerInvariant() == "config")
        {
            _service.Configure();
            return;
        }
        try
        {
            var responses = await _service.LookupDictionaryFor(word);
            if (responses is null || responses.Count() == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No results found for {word}[/]");
                return;
            }

            foreach (var response in responses)
            {
                AnsiConsoleHelper.TitleRule($":input_latin_letters: {response.Id} <{response.PartOfSpeech}>");
                foreach (var alternative in response.Alternatives)
                {
                    AnsiConsole.MarkupLine($"[cyan]Definition:[/] [white]{alternative.ShortDefinition}[/]");
                }
                AnsiConsole.WriteLine();
            }
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
    }

    private async Task LookupThesaurusFor(string word)
    {
        if (word.ToLowerInvariant() == "config")
        {
            _service.Configure();
            return;
        }
        try
        {
            var responses = await _service.LookupThesaurusFor(word);
            if (responses is null || responses.Count() == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No results found for {word}[/]");
                return;
            }

            foreach (var response in responses)
            {
                AnsiConsoleHelper.TitleRule($":input_latin_letters: {response.Id} <{response.PartOfSpeech}>");
                foreach (var alternative in response.Alternatives)
                {
                    AnsiConsole.MarkupLine($"[cyan]Definition:[/] [white]{alternative.ShortDefinition}[/]");
                    AnsiConsole.MarkupLine($"[cyan]Synonyms:[/] [green]{string.Join(", ", alternative.Synonyms)}[/]");
                    AnsiConsole.MarkupLine($"[cyan]Antonyms:[/] [yellow]{string.Join(", ", alternative.Antonyms)}[/]");
                    AnsiConsole.WriteLine();
                }
            }
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
    }
}
