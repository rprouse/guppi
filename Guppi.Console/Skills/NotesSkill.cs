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
            var notes = new Command("notes", "Opens the notes directory and creates a new note for today if it doesn't exist.")
            {
                new Option<bool>(new string[]{"--nocreate", "-n"}, "Do not create today's note."),
                new Option<bool>(new string[]{"--configure", "-c"}, "Set the notes directory."),
                new Option<string>( new string[]{"--title", "-t"}, () => string.Empty, "Sets the note title")
            };
            notes.AddArgument(new Argument<string>("filename", () => DateTime.Now.ToString("yyyy-MM-dd")));
            notes.Handler = CommandHandler.Create(async (string title, bool nocreate, bool configure, string filename) => 
                await OpenNotes(title, nocreate, configure, filename));
            return new[] { notes };
        }

        private async Task OpenNotes(string title, bool nocreate, bool configure, string filename)
        {
            if (configure)
                await _mediator.Send(new ConfigureNotesCommand());

            await _mediator.Send(new OpenNotesCommand(title, filename, nocreate));
        }
    }
}
