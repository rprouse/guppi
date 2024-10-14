namespace Guppi.Domain.Interfaces
{
    public interface IGitService
    {
        void SwitchToBranch(string branchName);
        void RunGit(string args);
    }
}
