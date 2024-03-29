using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Application.Services;

public interface ISerialPortService
{
    IEnumerable<string> GetPorts();
    Task UploadHex(string port, int baud, string filename, Action<string> write);
}
