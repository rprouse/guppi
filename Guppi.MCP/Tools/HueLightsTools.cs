using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Services.Hue;
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

        [McpServerTool, Description("Lists all Philips Hue lights and their current state (on/off, brightness, color)")]
        public async Task<object> ListHueLights(
            [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null)
        {
            try
            {
                return await _service.ListLights(ip, _ => { });
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

        [McpServerTool, Description("Turns on a Philips Hue light with optional brightness and color")]
        public async Task<object> TurnOnHueLight(
            [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null,
            [Description("Light ID to target. 0 for all lights. Leave empty for default light.")] uint light = 0,
            [Description("Brightness percentage, 0-100.")] byte? brightness = null,
            [Description("Color as hex (FF0000 or #FF0000) or named color (red, blue).")] string color = null)
        {
            try
            {
                uint targetLight = light == 0 ? _service.GetDefaultLight() : light;
                var command = new SetLightCommand
                {
                    IpAddress = ip,
                    On = true,
                    Off = false,
                    Alert = false,
                    Brightness = brightness,
                    Color = color,
                    Light = targetLight,
                    WaitForUserInput = _ => { }
                };
                await _service.SetLight(command);
                return await _service.ListLights(ip, _ => { });
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
