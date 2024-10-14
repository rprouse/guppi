namespace Guppi.Core.Interfaces.Providers
{
    public interface IGitProvider
    {
        void SwitchToBranch(string branchName);
        void RunGit(string args);
    }
}
