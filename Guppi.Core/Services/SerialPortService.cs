using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

internal class SerialPortService : ISerialPortService
{
    public IEnumerable<string> GetPorts()
    {
        if (OperatingSystem.IsWindows())
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var result = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
            return result.Get()
                .Cast<ManagementBaseObject>()
                .OrderBy(p => p["DeviceId"])
                .Select(p => $"{p["Manufacturer"]} {p["Name"]}");
#pragma warning restore CA1416 // Validate platform compatibility
        }
        return SerialPort.GetPortNames();
    }

    public async Task UploadHex(string port, int baud, string filename, Action<string> write)
    {
        var lines = File.ReadAllLines(filename);

        using var serialPort = new SerialPort(port, baud);
        serialPort.Open();
        
        foreach (var line in lines)
        {
            var bytes = Encoding.ASCII.GetBytes(line);
            serialPort.Write(bytes, 0, bytes.Length);
            write(line);
            await Task.Delay(10);
        }
    }
}
