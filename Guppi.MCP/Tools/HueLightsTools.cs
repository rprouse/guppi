using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
    [McpServerToolType]
    public class HueLightsTools
    {
        private readonly IHueLightService _service;

        public HueLightsTools(IHueLightService service)
        {
            _service = service;
        }

        [McpServerTool, Description("Lists Philips Hue bridges found on the local network")]
        public async Task<object> ListHueBridges()
        {
            try
            {
                return await _service.ListBridges();
            }
            catch (ArgumentException)
            {
                return "Error: Hue Bridge not found";
            }
            catch (InvalidOperationException)
            {
                return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
