using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Guppi.Core;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;

namespace Guppi.Infrastructure.Services.Hue
{
    internal partial class HueProvider : IHueProvider
    {
        string _key;
        ILocalHueClient _client;
        readonly RGBColor _black = new ("000000");

        public Action<string> WaitForUserInput { get; set; } = null;

        public async Task<IEnumerable<HueBridge>> ListBridges()
        {
            var locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            return bridges.Select(b => new HueBridge { BridgeId = b.BridgeId, IpAddress = b.IpAddress });
        }

        public async Task<IEnumerable<HueLight>> ListLights(string ip)
        {
            if (await ConnectToBridge(ip) == false)
                return Enumerable.Empty<HueLight>();

            var lights = await _client.GetLightsAsync();
            return lights.Select(l => new HueLight { Id = l.Id, Name = l.Name, On = l.State.On, Brightness = l.State.Brightness, Color = l.ToHex(), Type = l.Type });
        }

        public async Task Set(string ip, bool on, bool off, bool alert, byte? brightness, string color, uint light)
        {
            if (await ConnectToBridge(ip) == false)
                return;

            var cmd = GetCommand(on, off, alert, brightness, color);
            var lts = GetLights(light);
            await SendCommand(cmd, lts);
        }

        public async Task<bool> Register(string ip = null)
        {
            return await ConnectToBridge(ip, false);
        }

        public async Task<bool> ConnectToBridge(string ip = null, bool loadKey = true)
        {
            var locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            LocatedBridge bridge = null;
            if (ip == null)
                bridge = bridges.FirstOrDefault();
            else
                bridge = bridges.FirstOrDefault(b => b.IpAddress == ip);

            if (bridge == null)
            {
                throw new ArgumentException("Hue Bridge not found");
            }

            var configuration = new HueKey(bridge.IpAddress);

            if (loadKey)
                _key = configuration.LoadKey();

            if (_key == null)
            {
                _key = await Register(bridge);
                if (_key == null)
                    return false;

                if (!configuration.SaveKey(_key))
                    return false;
            }

            _client = Initialize(bridge);
            return true;
        }

        async Task<string> Register(LocatedBridge bridge)
        {
            if (WaitForUserInput == null)
                throw new InvalidOperationException("You must set WaitForUserInput");

            WaitForUserInput("Press the button on your bridge then press ENTER");
            try
            {
                var client = new LocalHueClient(bridge.IpAddress);
                return await client.RegisterAsync("Alteridem.Hue.CLI", Environment.MachineName);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        ILocalHueClient Initialize(LocatedBridge bridge)
        {
            var client = new LocalHueClient(bridge.IpAddress);
            client.Initialize(_key);
            return client;

        }

        LightCommand GetCommand(bool on, bool off, bool alert, byte? brightness, string color)
        {
            var command = new LightCommand();
            if (on)
                command.On = true;
            else if (off)
                command.On = false;

            if (alert)
                command.Alert = Alert.Multiple;

            if (brightness.HasValue)
            {
                double p = (brightness.Value <= 100 ? brightness.Value : 100d) / 100d;
                byte b = (byte)(p * 255);
                command.Brightness = b;
            }

            if (color != null)
            {
                RGBColor rgbColor = GetColor(color);
                if (rgbColor != _black)
                    command.SetColor(rgbColor);
            }

            return command;
        }

        static IEnumerable<string> GetLights(uint light)
        {
            if (light > 0)
                return new[] { light.ToString() };

            return null;
        }

        Task<HueResults> SendCommand(LightCommand command, IEnumerable<string> lights = null) =>
            _client.SendCommandAsync(command, lights);

        static RGBColor GetColor(string color)
        {
            // Is it an RGB Color?
            var r = ColorRegex();
            if (r.IsMatch(color))
                return new RGBColor(color);

            // Maybe it is a named color
            var c = System.Drawing.Color.FromName(color);
            return new RGBColor(c.R, c.G, c.B);
        }

        [GeneratedRegex("^#?[0-9a-fA-F]{6}$", RegexOptions.CultureInvariant)]
        private static partial Regex ColorRegex();
    }
}
