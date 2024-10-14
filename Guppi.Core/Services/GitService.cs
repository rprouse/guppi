namespace Guppi.Application.Services;

internal sealed class GitService : IGitService
{
    private readonly Guppi.Domain.Interfaces.IGitService _gitService;

    public GitService(Domain.Interfaces.IGitService gitService)
    {
        _gitService = gitService;
    }

    public void Ammend()
    {
        _gitService.RunGit("commit --amend --no-edit");
    }

    public void Undo(bool hard)
    {
        _gitService.RunGit(string.Format("reset --{0} HEAD~1", hard ? "hard" : "soft"));
    }

    public void Unstage()
    {
        _gitService.RunGit("reset -q HEAD --");
    }

    public void Update(string branch)
    {
        _gitService.SwitchToBranch(branch);
        _gitService.RunGit("fetch -p");
        _gitService.RunGit("pull");
    }
}
