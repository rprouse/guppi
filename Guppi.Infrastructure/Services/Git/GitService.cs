using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using LibGit2Sharp;

namespace Guppi.Infrastructure.Services.Git
{
    public class GitService : IGitService
    {
        const string WorkingDirectory = ".";
        private readonly IProcessService _process;

        public GitService(IProcessService process)
        {
            _process = process;
        }

        public void RunGit(string args)
        {
            _process.Start("git", args, WorkingDirectory, true);
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
