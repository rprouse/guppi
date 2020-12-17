using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColoredConsole;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;

namespace DataProvider.Hue
{
    internal class HueController
    {
        string _key;
        ILocalHueClient _client;
        readonly RGBColor _black = new RGBColor("000000");

        public async Task ListBridges()
        {
            ColorConsole.WriteLine("[SUDAR Scan Complete. Found bridges...]".White());

            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            foreach (var bridge in bridges)
            {
                ColorConsole.WriteLine($"{bridge.BridgeId} - {bridge.IpAddress}".Gray());
            }
        }

        public async Task<bool> Register(string ip = null)
        {
            return await ConnectToBridge(ip, false);
        }

        public async Task<bool> ConnectToBridge(string ip = null, bool loadKey = true)
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            LocatedBridge bridge = null;
            if (ip == null)
                bridge = bridges.FirstOrDefault();
            else
                bridge = bridges.FirstOrDefault(b => b.IpAddress == ip);

            if (bridge == null)
            {
                ColorConsole.WriteLine("[Hue Bridge not found]".Red());
                return false;
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
            ColorConsole.WriteLine("[Press the button on your bridge then press ENTER]".Green());
            Console.ReadLine();

            try
            {
                ILocalHueClient client = new LocalHueClient(bridge.IpAddress);
                return await client.RegisterAsync("Alteridem.Hue.CLI", Environment.MachineName);
            }
            catch (Exception e)
            {
                ColorConsole.WriteLine("[Failed to register with the bridge.]".Red());
                ColorConsole.WriteLine(e.Message.Red());
            }
            return null;
        }

        ILocalHueClient Initialize(LocatedBridge bridge)
        {
            var client = new LocalHueClient(bridge.IpAddress);
            client.Initialize(_key);
            return client;

        }

        public async Task ListLights()
        {
            ColorConsole.WriteLine("Found lights...".White());
            var lights = await _client.GetLightsAsync();
            foreach (var light in lights)
            {
                ColorConsole.WriteLine($"{light.Id} - {light.Name} ({(light.State.On ? $"On {(light.State.Brightness * 100 / 255)}%" : "Off")})".Gray());
            }
        }

        public LightCommand GetCommand(bool on, bool off, bool alert, byte? brightness, string color)
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

        public IEnumerable<string> GetLights(uint light)
        {
            if (light > 0)
                return new[] { light.ToString() };

            return null;
        }

        public Task<HueResults> SendCommand(LightCommand command, IEnumerable<string> lights = null) =>
            _client.SendCommandAsync(command, lights);

        RGBColor GetColor(string color)
        {
            // Is it an RGB Color?
            var r = new Regex("^#?[0-9a-fA-F]{6}$", RegexOptions.CultureInvariant);
            if (r.IsMatch(color))
                return new RGBColor(color);

            // Maybe it is a named color
            Color c = Color.FromName(color);
            return new RGBColor(c.R, c.G, c.B);
        }
    }
}
