using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Guppi.Core.Interfaces.Providers;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

public class IPService(IHttpRestProvider http) : IIPService
{
    private const string RequestUri = "http://icanhazip.com";

    private readonly IHttpRestProvider _http = http;

    public async Task<IPAddress> GetWanIPAddress()
    {
        var ipStr = await _http.GetStringAsync(RequestUri);
        ipStr = ipStr.Replace("\\r\\n", "").Replace("\\n", "").Trim();
        if (!IPAddress.TryParse(ipStr, out var ipAddress)) return null;
        return ipAddress;
    }

    public IList<NetworkInterface> GetNetworkInterfaces() => 
        NetworkInterface.GetAllNetworkInterfaces();
}
