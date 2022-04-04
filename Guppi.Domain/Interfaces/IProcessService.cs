namespace Guppi.Domain.Interfaces
{
    public interface IProcessService
    {
        /// <summary>
        /// Launches a Process
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="arguments"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="waitForExit"></param>
        public void Start(string filename, string arguments = "", string workingDirectory = ".", bool waitForExit = false);

        /// <summary>
        /// Opens a URI
        /// </summary>
        /// <param name="uri"></param>
        public void Open(string uri);
    }
}
