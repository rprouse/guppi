using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Commands.Notes;
using MediatR;

namespace ActionProvider.Notes
{
    public class NotesProvider : IActionProvider
    {
        private readonly IMediator _mediator;

        public NotesProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Command GetCommand()
        {
            var notes = new Command("notes", "Opens the notes directory and creates a new note for today if it doesn't exist.")
            {
                new Option<bool>(new string[]{"--nocreate", "-n"}, "Do not create today's note."),
                new Option<bool>(new string[]{"--configure", "-c"}, "Set the notes directory.")
            };
            notes.AddArgument(new Argument<string>("filename", () => DateTime.Now.ToString("yyyy-MM-dd")));
            notes.Handler = CommandHandler.Create(async (bool nocreate, bool configure, string filename) => await OpenNotes(nocreate, configure, filename));
            return notes;
        }

        private async Task OpenNotes(bool nocreate, bool configure, string filename)
        {
            if (configure)
                await _mediator.Send(new ConfigureNotesCommand());

            await _mediator.Send(new OpenNotesCommand());
        }
    }
}
