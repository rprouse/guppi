namespace Guppi.Domain.Interfaces
{
    public interface IProcessService
    {
        public void Start(string filename, string arguments = "", string workingDirectory = ".", bool waitForExit = false);
    }
}
