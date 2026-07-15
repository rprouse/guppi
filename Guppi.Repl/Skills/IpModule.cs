using Repl;
using Repl.Mcp;

namespace Guppi.Repl.Skills
{

    public sealed class IpModule(ILocalIpAddressProvider addresses) : IReplModule
    {
        public void Map(IReplMap map)
        {
            map.Map("local", () => addresses.GetLocalAddresses())
                .WithDescription("List active local IPv4 addresses")
                .ReadOnly();
        }
    }
}
