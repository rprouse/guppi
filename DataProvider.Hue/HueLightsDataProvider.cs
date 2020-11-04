using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Guppi.Core;

namespace DataProvider.Hue
{
    public class HueLightsDataProvider : IDataProvider
    {
        HueConfiguration _configuration;

        public HueLightsDataProvider()
        {
            _configuration = Configuration.Load<HueConfiguration>("hue");
        }

        public Command GetCommand()
        {
            var bridges = new Command("bridges", "List bridges on the network");
            bridges.Handler = CommandHandler.Create(async () => await ListBridges());

            var configure = new Command("configure", "Configures default lights");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(() => Configure());

            var register = new Command("register", "Register with a Hue Bridge. Registration usually happens automatically, you should only need to use to fix a broken registration");
            register.Handler = CommandHandler.Create(async (string ip) => await Register(ip));

            var lights = new Command("list", "List lights");
            lights.AddAlias("ls");
            lights.Handler = CommandHandler.Create(async (string ip) => await ListLights(ip));

            var on = new Command("on", "Turn lights on")
            {
                new Option<byte?>(new string[]{ "--brightness", "-b" }, "Set the brightness of a light, from 0 to 100 percent"),
                new Option<string>(new string[]{ "--color", "-c" }, "Color as a HEX color in the format FF0000 or #FF0000, or a common color name like red or blue"),
                new Option<uint>(new string[]{ "--light", "-l" }, () => _configuration.GetDefaultLight(), "The light to perform an action on. If unset, your default light or if 0 all lights"),
            };
            on.Handler = CommandHandler.Create(async (string ip, byte? brightness, string color, uint light) => await Set(ip, true, false, false, brightness, color, light));

            var off = new Command("off", "Turn lights off")
            {
                new Option<uint>(new string[]{ "--light", "-l" }, () => _configuration.GetDefaultLight(), "The light to perform an action on. If unset, your default light or if 0 all lights"),
            };
            off.Handler = CommandHandler.Create(async (string ip, uint light) => await Set(ip, false, true, false, null, null, light));

            var alert = new Command("alert", "Set an alert on the lights")
            {
                new Option<byte?>(new string[]{ "--brightness", "-b" }, "Set the brightness of a light, from 0 to 100 percent"),
                new Option<string>(new string[]{ "--color", "-c" }, "Color as a HEX color in the format FF0000 or #FF0000, or a common color name like red or blue"),
                new Option<uint>(new string[]{ "--light", "-l" }, () => _configuration.GetDefaultLight(), "The light to perform an action on. If unset, your default light or if 0 all lights"),
            };
            alert.Handler = CommandHandler.Create(async (string ip, byte? brightness, string color, uint light) => await Set(ip, false, false, true, brightness, color, light));

            var set = new Command("set", "Sets the brightness and/or color to a light or lights")
            {
                new Option<byte?>(new string[]{ "--brightness", "-b" }, "Set the brightness of a light, from 0 to 100 percent"),
                new Option<string>(new string[]{ "--color", "-c" }, "Color as a HEX color in the format FF0000 or #FF0000, or a common color name like red or blue"),
                new Option<uint>(new string[]{ "--light", "-l" }, () => _configuration.GetDefaultLight(), "The light to perform an action on. If unset, your default light or if 0 all lights"),
            };
            set.Handler = CommandHandler.Create(async (string ip, byte? brightness, string color, uint light) => await Set(ip, false, false, false, brightness, color, light));

            var command = new Command("hue", "Works with Philips Hue Lights")
            {
               bridges,
               configure,
               register,
               lights,
               on,
               off,
               alert,
               set
            };
            command.AddAlias("lights");

            command.AddOption(new Option<string>(new string[] { "--ip", "-i" }, "IP Address of the Hue Bridge. Will default to the first bridge found."));
            return command;
        }

        private async Task ListBridges()
        {
            var controller = new HueController();
            await controller.ListBridges();
        }

        private async Task ListLights(string ip)
        {
            var controller = new HueController();
            if (await controller.ConnectToBridge(ip) == false)
                return;

            await controller.ListLights();
        }

        private async Task Set(string ip, bool on, bool off, bool alert, byte? brightness, string color, uint light)
        {
            var controller = new HueController();
            if (await controller.ConnectToBridge(ip) == false)
                return;

            var cmd = controller.GetCommand(on, off, alert, brightness, color);
            var lts = controller.GetLights(light);
            await controller.SendCommand(cmd, lts);
        }

        private async Task Register(string ip)
        {
            var controller = new HueController();
            await controller.Register(ip);
        }

        private void Configure()
        {
            _configuration.RunConfiguration("Hue Lights", "Enter your default light");
        }
    }
}
