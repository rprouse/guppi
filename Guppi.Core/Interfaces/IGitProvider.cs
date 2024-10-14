namespace Guppi.Core.Interfaces
{
    public interface IGitProvider
    {
        void SwitchToBranch(string branchName);
        void RunGit(string args);
    }
}
