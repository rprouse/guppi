using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Guppi.Application.Exceptions;
using Guppi.Application.Services;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    public class GitSkill : ISkill
    {
        private readonly IGitService _service;

        public GitSkill(IGitService service)
        {
            _service = service;
        }

        public IEnumerable<Command> GetCommands()
        {
            var amend = new Command("ammend", "Adds any staged files to the last git commit using the same message");
            amend.Handler = CommandHandler.Create(() => Ammend());

            var undo = new Command("undo", "Undo the last git commit. By default, does a soft reset preserving changes.")
            {
                new Option<bool>(new string[]{"--hard", "-h"}, () => false, "Do a hard reset not preserving changes.")
            };
            undo.Handler = CommandHandler.Create((bool hard) => Undo(hard));

            var unstage = new Command("unstage", "Unstages any staged files.");
            unstage.Handler = CommandHandler.Create(() => Unstage());

            var update = new Command("update", "Switches to the default git branch, fetches and pulls")
            {
                new Option<string>(new string[]{"--branch", "-b" }, () => "master", "The branch to switch to. Defaults to master."),
            };
            update.Handler = CommandHandler.Create((string branch) => Update(branch));

            return new[] 
            {
                new Command("git", "Helpful git extensions")
                {
                    amend,
                    undo,
                    unstage,
                    update,
                }
            };
        }

        void Update(string branchName)
        {
            try
            {
                AnsiConsole.MarkupLine($"[green][[:check_mark_button: Switching to branch {branchName}]][/]");
                _service.Update(branchName);
            }
            catch(WarningException ex)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ex.Message}]][/]");
            }
            catch(UnconfiguredException)
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: The current directory is not a git repository.]][/]");
                AnsiConsole.MarkupLine("[silver][[Make sure you are in the root of the repository.]][/]");
            }
        }

        void Ammend() => _service.Ammend();

        void Undo(bool hard) => _service.Undo(hard);

        void Unstage() => _service.Unstage();
    }
}
