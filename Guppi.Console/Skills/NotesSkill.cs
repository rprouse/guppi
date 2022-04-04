using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Guppi.Application.Commands.Notes;
using MediatR;

namespace Guppi.Console.Skills
{
    public class NotesSkill : ISkill
    {
        private readonly IMediator _mediator;

        public NotesSkill(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEnumerable<Command> GetCommands()
        {
            var add = new Command("add", "Adds a new note.")
            {
                new Option<string>( new string[]{"--title", "-t"}, "Sets the note title"),
                //new Option<string>( new string[]{"--folder", "-f"}, () => string.Empty, "Sub-folder for the note"),
                new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            };
            add.Handler = CommandHandler.Create(async (string title, /* string folder, */ string vault) => await Add(title, null, vault));

            //var daily = new Command("daily", "Adds a new daily note")
            //{
            //    new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            //};
            //daily.Handler = CommandHandler.Create(async (string vault) => await Daily(vault));

            var view = new Command("view", "Opens Obsidian")
            {
                new Option<string>( new string[]{"--vault", "-v"}, () => string.Empty, "Use a vault other than the default")
            };
            view.AddAlias("open");
            view.Handler = CommandHandler.Create(async (string vault) => await View(vault));

            var code = new Command("code", "Opens the notes folder in VS Code");
            code.Handler = CommandHandler.Create(async () => await Code());

            var config = new Command("configure", "Configure the notes skill.");
            config.AddAlias("config");
            config.Handler = CommandHandler.Create(async () => await Configure());

            var notes = new Command("notes", "Works with Notes in Obsidian")
            {
                add,
                // daily,
                view,
                code,
                config
            };
            return new[] { notes };
        }

        private async Task Add(string title, string folder, string vault) =>
            await _mediator.Send(new AddFileCommand(title, folder, vault));

        private async Task Daily(string vault)
        {
            // TODO
        }

        private async Task View(string vault) =>
            await _mediator.Send(new OpenObsidianCommand(vault, null));

        private async Task Code() => 
            await _mediator.Send(new OpenCodeCommand());

        private async Task Configure() => 
            await _mediator.Send(new ConfigureNotesCommand());
    }
}
