using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace Guppi.Application.Services;

internal class SerialPortService : ISerialPortService
{
    public IEnumerable<string> GetPorts() => 
        SerialPort.GetPortNames();

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
