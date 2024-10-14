using System.Diagnostics;
using System.Linq;
using Guppi.Core.Interfaces;

namespace Guppi.Infrastructure.Services
{
    public class ProcessProvider : IProcessProvider
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

        public void Open(string uri)
        {
            var ps = new ProcessStartInfo(uri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        public void Kill(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach(var process in processes)
            {
                process.Kill();
            }
        }

        public bool Running(string processName) =>
            Process.GetProcessesByName(processName).Any();
    }
}
