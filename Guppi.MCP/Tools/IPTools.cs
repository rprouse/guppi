using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Guppi.Core.Interfaces.Services;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
    [McpServerToolType]
    public class IPTools
    {
        private readonly IIPService _service;

        public IPTools(IIPService service)
        {
            _service = service;
        }

        [McpServerTool, Description("Gets the public (WAN) IP address of the current machine")]
        public async Task<object> GetWanIpAddress()
        {
            try
            {
                var ip = await _service.GetWanIPAddress();
                if (ip == null)
                    return "Error: Could not determine WAN IP address";

                return new { WanIpAddress = ip.ToString() };
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [McpServerTool, Description("Lists all local network interfaces and their IPv4 addresses")]
        public Task<object> GetLocalIpAddresses()
        {
            try
            {
                var interfaces = _service.GetNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(n => n.GetIPProperties().UnicastAddresses
                        .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(a => new
                        {
                            InterfaceName = n.Name,
                            IpAddress = a.Address.ToString()
                        }))
                    .ToList();

                if (interfaces.Count == 0)
                    return Task.FromResult<object>("No active IPv4 network interfaces found");

                return Task.FromResult<object>(interfaces);
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>($"Error: {ex.Message}");
            }
        }
    }
}
