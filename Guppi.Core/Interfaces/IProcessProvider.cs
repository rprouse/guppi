namespace Guppi.Core.Interfaces
{
    public interface IProcessProvider
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

        /// <summary>
        /// Kills a process
        /// </summary>
        /// <param name="processName">The name of the process to kill</param>
        /// <returns>True if the process is killed.</returns>
        public void Kill(string processName);

        /// <summary>
        /// Determines if a process is running
        /// </summary>
        /// <param name="processName">The process</param>
        /// <returns>True if the process is running</returns>
        public bool Running(string processName);
    }
}
