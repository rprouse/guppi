using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Extensions;
using Guppi.Core.Services.Hue;
using Guppi.Core.Entities.Hue;
using Spectre.Console;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Console.Skills;

internal class HueLightsSkill : ISkill
{
    private readonly IHueLightService _service;

    public HueLightsSkill(IHueLightService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        uint defaultLight = _service.GetDefaultLight();

        // This is a workaround for https://github.com/dotnet/command-line-api/issues/1683, remove at next System.Commandline release
        ParseArgument<uint> lightParser = (ArgumentResult result) =>
        {
            if(result.Tokens.Any())
            {
                var value = result.Tokens.First().Value;
                if (uint.TryParse(value, out uint light))
                {
                    return light;
                }
                result.ErrorMessage = $"Error: {value} is not an unsigned integer";
            }
            return defaultLight;
        };

        ParseArgument<byte?> brightParser = (ArgumentResult result) =>
        {
            if (result.Tokens.Any())
            {
                var value = result.Tokens.First().Value;
                if (byte.TryParse(value, out byte brightness))
                {
                    return brightness;
                }
                result.ErrorMessage = $"Error: {value} is not a byte";
            }
            return null;
        };

        // Common options
        var brightness = new Option<byte?>(new string[] { "--brightness", "-b" }, brightParser, description: "Set the brightness of a light, from 0 to 100 percent");
        var color = new Option<string>(new string[] { "--color", "-c" }, "Color as a HEX color in the format FF0000 or #FF0000, or a common color name like red or blue");
        var light = new Option<uint>(new string[] { "--light", "-l" }, lightParser, description: "The light to perform an action on. If unset, your default light or if 0 all lights");
        light.SetDefaultValue(defaultLight);

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
            brightness,
            color,
            light,
        };
        on.Handler = CommandHandler.Create(async (string ip, byte? brightness, string color, uint light) => await Set(ip, true, false, false, brightness, color, light));

        var off = new Command("off", "Turn lights off")
        {
            light,
        };
        off.AddAlias("out");
        off.Handler = CommandHandler.Create(async (string ip, uint light) => await Set(ip, false, true, false, null, null, light));

        var alert = new Command("alert", "Set an alert on the lights")
        {
            brightness,
            color,
            light,
        };
        alert.Handler = CommandHandler.Create(async (string ip, byte? brightness, string color, uint light) => await Set(ip, false, false, true, brightness, color, light));

        var set = new Command("set", "Sets the brightness and/or color to a light or lights")
        {
            brightness,
            color,
            light,
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
        return new[] { command };
    }

    private async Task ListBridges()
    {
        try
        {
            var bridges = await _service.ListBridges();
            AnsiConsoleHelper.TitleRule(":desktop_computer: SUDAR Scan Complete. Found bridges...");
            foreach (var bridge in bridges)
            {
                AnsiConsole.MarkupLine($"[silver]{bridge.BridgeId} - {bridge.IpAddress}[/]");
            }
            AnsiConsoleHelper.Rule("white");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red][[:cross_mark: Failed to list bridges.]][/]");
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }

    private async Task ListLights(string ip)
    {
        try
        {
            var lights = await _service.ListLights(ip, WaitForUserInput);
            if (lights.Count() == 0)
            {
                AnsiConsole.MarkupLine("[red][[:cross_mark: No lights found.]][/]");
                return;
            }
            AnsiConsoleHelper.TitleRule(":light_bulb: Scans are complete. Found lights...");
            OutputLights(lights);
        }
        catch (ArgumentException ae)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: {ae.Message}]][/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine("[red][[:cross_mark: Failed to register with the bridge.]][/]");
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }

    private static void OutputLights(IEnumerable<HueLight> lights)
    {
        foreach (var light in lights)
        {
            AnsiConsole.MarkupLine($"[white]{light.Id,2}: {light.Name,-40}[/] [#{light.Color}]{(light.On ? $"● {(light.Brightness * 100 / 255)}% #{light.Color}" : "○")}[/]");
        }
        AnsiConsoleHelper.Rule("white");
    }

    private async Task Set(string ip, bool on, bool off, bool alert, byte? brightness, string color, uint light)
    {
        try
        {
            var command = new SetLightCommand { IpAddress = ip, On = on, Off = off, Alert = alert, Brightness = brightness, Color = color, Light = light, WaitForUserInput = WaitForUserInput };
            await _service.SetLight(command);

            AnsiConsoleHelper.TitleRule(":light_bulb: Roamer systems dispatched. Lights have been adjusted...");

            var lights = await _service.ListLights(ip, WaitForUserInput);
            OutputLights(lights);
        }
        catch (ArgumentException ae)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: {ae.Message}]][/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine("[red][[:cross_mark: Failed to register with the bridge.]][/]");
            AnsiConsole.WriteException(ex,
                ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }

    private async Task Register(string ip) =>
        await _service.Register(ip, WaitForUserInput);

    private void WaitForUserInput(string message)
    {
        AnsiConsole.MarkupLine($"[green][[:check_mark_button: {message}]][/]");
        System.Console.ReadLine();
    }

    private void Configure() => _service.Configure();
}
