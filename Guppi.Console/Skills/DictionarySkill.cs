using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Application.Services;

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
        var configure = new Command("configure", "Configure calendars");
        configure.AddAlias("config");
        configure.SetHandler(() => Configure());

        var dict = new Command("dictionary", "Displays the meanings of words.")
        {
            configure,
        };
        dict.AddAlias("dict");

        var thes = new Command("thesaurus", "Displays synonyms of words.")
        {
            configure,
        };
        thes.AddAlias("synonyms");
        thes.AddAlias("syn");

        return new[] { dict, thes };
    }

    private void Configure() => _service.Configure();
}
