using System.Diagnostics;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using LibGit2Sharp;

namespace Guppi.Infrastructure.Services.Git
{
    public class GitService : IGitService
    {
        const string WorkingDirectory = ".";
        //const string WorkingDirectory = @"C:\Src\Reliq\aws-cli-s3-deploy";

        public void RunGit(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = WorkingDirectory,
                UseShellExecute = false
            };
            var test = Process.Start(psi);
            test.WaitForExit();
        }

        public void SwitchToBranch(string branchName)
        {
            try
            {
                using (var repo = new Repository(WorkingDirectory))
                {
                    var branch = repo.Branches[branchName];
                    if (branch == null)
                    {
                        throw new WarningException($"The branch {branchName} does not exist.");
                    }

                    Commands.Checkout(repo, branch);
                }
            }
            catch (RepositoryNotFoundException)
            {
                throw new UnconfiguredException("The current directory is not a git repository.");
            }
        }
    }
}
