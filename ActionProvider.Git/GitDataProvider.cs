using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using Spectre.Console;
using Guppi.Core;
using LibGit2Sharp;

namespace ActionProvider.Git
{
    public class GitDataProvider : IActionProvider
    {
        const string WorkingDirectory = ".";
        //const string WorkingDirectory = @"C:\Src\Reliq\aws-cli-s3-deploy";

        public Command GetCommand()
        {
            var amend = new Command("ammend", "Adds any steged files to the last git commit using the same message");
            amend.Handler = CommandHandler.Create(() => RunGit("commit --amend --no-edit"));

            var undo = new Command("undo", "Undo the last git commit. By default, does a soft reset preserving changes.")
            {
                new Option<bool>(new string[]{"--hard", "-h"}, () => false, "Do a hard reset not preserving changes.")
            };
            undo.Handler = CommandHandler.Create((bool hard) => RunGit(string.Format("reset --{0} HEAD~1", hard ? "hard" : "soft")));

            var unstage = new Command("unstage", "Unstages any staged files.");
            unstage.Handler = CommandHandler.Create(() => RunGit("reset -q HEAD --"));

            var update = new Command("update", "Switches to the default git branch, fetches and pulls")
            {
                new Option<string>(new string[]{"--branch", "-b" }, () => "master", "The branch to switch to. Defaults to master."),
            };
            update.Handler = CommandHandler.Create((string branch) => Update(branch));

            return new Command("git", "Helpful git extensions")
            {
                amend,
                undo,
                unstage,
                update,
            };
        }

        void Update(string branchName)
        {
            try
            {
                using (var repo = new Repository(WorkingDirectory))
                {
                    var branch = repo.Branches[branchName];
                    if(branch == null)
                    {
                        AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: The branch {branchName} does not exist.]][/]");
                        return;
                    }

                    Branch currentBranch = Commands.Checkout(repo, branch);
                    AnsiConsole.MarkupLine($"[green][[:check_mark_button: Switched to branch {currentBranch.FriendlyName}]][/]");
                }

                RunGit("fetch -p");
                RunGit("pull");
            }
            catch(RepositoryNotFoundException)
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: The current directory is not a git repository.]][/]");
                AnsiConsole.MarkupLine("[silver][[Make sure you are in the root of the repository.]][/]");
            }
        }

        void RunGit(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = WorkingDirectory,
                UseShellExecute = false
            };
            var test = System.Diagnostics.Process.Start(psi);
            test.WaitForExit();
        }
    }
}
