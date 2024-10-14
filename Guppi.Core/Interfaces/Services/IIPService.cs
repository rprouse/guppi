using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IIPService
{
    IList<NetworkInterface> GetNetworkInterfaces();

    Task<IPAddress> GetWanIPAddress();
}
