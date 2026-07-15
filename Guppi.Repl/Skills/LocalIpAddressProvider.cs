using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Guppi.Core.Interfaces.Services;
using Guppi.Repl.Results;

namespace Guppi.Repl.Skills
{

    public interface ILocalIpAddressProvider
    {
        LocalIpAddressResult[] GetLocalAddresses();
    }

    public sealed class LocalIpAddressProvider(IIPService service) : ILocalIpAddressProvider
    {
        public LocalIpAddressResult[] GetLocalAddresses() =>
            service.GetNetworkInterfaces()
                .Where(network => network.OperationalStatus == OperationalStatus.Up)
                .SelectMany(network => network.GetIPProperties().UnicastAddresses
                    .Where(address => address.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(address => new LocalIpAddressResult(network.Name, address.Address.ToString())))
                .ToArray();
    }
}
