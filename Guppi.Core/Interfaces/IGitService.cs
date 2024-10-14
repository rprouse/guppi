namespace Guppi.Core.Interfaces
{
    public interface IGitService
    {
        void SwitchToBranch(string branchName);
        void RunGit(string args);
    }
}
