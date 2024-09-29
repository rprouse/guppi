using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Guppi.Application.Services;

namespace Guppi.Console.Skills;

internal class NotesSkill(INoteService service) : ISkill
{
    private readonly INoteService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var add = new Command("add", "Adds a new note.")
        {
            new Option<string>( new string[]{"--title", "-t"}, "Sets the note title"),
            new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
        };
        add.Handler = CommandHandler.Create((string title, string vault) => Add(title, vault));

        var view = new Command("view", "Opens Obsidian")
        {
            new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
        };
        view.AddAlias("open");
        view.Handler = CommandHandler.Create((string vault) => View(vault));

        var code = new Command("code", "Opens the notes folder in VS Code")
        {
            Handler = CommandHandler.Create(() => Code())
        };

        var config = new Command("configure", "Configure the notes skill.");
        config.AddAlias("config");
        config.Handler = CommandHandler.Create(() => Configure());

        var notes = new Command("notes", "Works with Notes in Obsidian")
        {
            add,
            // daily,
            view,
            code,
            config
        };
        return [notes];
    }

    private void Add(string title, string vault) =>
        _service.AddFile(title, vault);

    private void View(string vault) =>
        _service.OpenObsidian(vault, null);

    private void Code() => 
        _service.OpenVsCode();

    private void Configure() =>
        _service.Configure();
}
