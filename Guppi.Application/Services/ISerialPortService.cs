using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guppi.Application.Services;

public interface ISerialPortService
{
    IEnumerable<string> GetPorts();
}

internal class SerialPortService : ISerialPortService
{
    public IEnumerable<string> GetPorts() => 
        SerialPort.GetPortNames();
}
