using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Guppi.Application.Commands.Git;
using Guppi.Application.Exceptions;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Actions
{
    public class GitDataProvider : IActionProvider
    {
        private readonly IMediator _mediator;

        public GitDataProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Command GetCommand()
        {
            var amend = new Command("ammend", "Adds any steged files to the last git commit using the same message");
            amend.Handler = CommandHandler.Create(async () => await Ammend());

            var undo = new Command("undo", "Undo the last git commit. By default, does a soft reset preserving changes.")
            {
                new Option<bool>(new string[]{"--hard", "-h"}, () => false, "Do a hard reset not preserving changes.")
            };
            undo.Handler = CommandHandler.Create(async (bool hard) => await Undo(hard));

            var unstage = new Command("unstage", "Unstages any staged files.");
            unstage.Handler = CommandHandler.Create(async () => await Unstage());

            var update = new Command("update", "Switches to the default git branch, fetches and pulls")
            {
                new Option<string>(new string[]{"--branch", "-b" }, () => "master", "The branch to switch to. Defaults to master."),
            };
            update.Handler = CommandHandler.Create(async (string branch) => await Update(branch));

            return new Command("git", "Helpful git extensions")
            {
                amend,
                undo,
                unstage,
                update,
            };
        }

        async Task Update(string branchName)
        {
            try
            {
                AnsiConsole.MarkupLine($"[green][[:check_mark_button: Switching to branch {branchName}]][/]");
                await _mediator.Send(new UpdateGitCommand { Branch = branchName });
            }
            catch(WarningException ex)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ex.Message}]][/]");
            }
            catch(UnconfiguredException ex)
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: The current directory is not a git repository.]][/]");
                AnsiConsole.MarkupLine("[silver][[Make sure you are in the root of the repository.]][/]");
            }
        }

        async Task Ammend() => await _mediator.Send(new AmmendGitCommand());

        async Task Undo(bool hard) => await _mediator.Send(new UndoGitCommand { Hard = hard });

        async Task Unstage() => await _mediator.Send(new UnstageGitCommand());
    }
}
