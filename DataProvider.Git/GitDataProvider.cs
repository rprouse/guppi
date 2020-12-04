using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using ColoredConsole;
using Guppi.Core;
using LibGit2Sharp;

namespace DataProvider.Git
{
    public class GitDataProvider : IDataProvider
    {
        const string WorkingDirectory = ".";
        //const string WorkingDirectory = @"C:\Src\Reliq\aws-cli-s3-deploy";

        public Command GetCommand()
        {
            var update = new Command("update", "Switches to the default git branch, fetches and pulls")
            {
                new Option<string>(new string[]{"--branch", "-b" }, () => "master", "The branch to switch to. Defaults to master."),
            };
            update.Handler = CommandHandler.Create((string branch) => Update(branch));

            return new Command("git", "Helpful git extensions")
            {
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
                        ColorConsole.WriteLine($"The branch {branchName} does not exist.".Yellow());
                        return;
                    }

                    Branch currentBranch = Commands.Checkout(repo, branch);
                    ColorConsole.WriteLine($"Switched to branch {currentBranch.FriendlyName}");
                }

                RunGit("fetch -p");
                RunGit("pull");
            }
            catch(RepositoryNotFoundException)
            {
                ColorConsole.WriteLine("The current directory is not a git repository.".Yellow());
                ColorConsole.WriteLine("Make sure you are in the root of the repository.".Cyan());
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
