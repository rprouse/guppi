using System.Diagnostics;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services
{
    public class ProcessService : IProcessService
    {
        public void Start(string filename, string arguments = "", string workingDirectory = ".", bool waitForExit = false)
        {
            var psi = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var process = Process.Start(psi);

            if(waitForExit)
                process.WaitForExit();
        }
    }
}
