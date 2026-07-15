using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Services.Hue;
using Guppi.Repl.Results;
using Repl;
using Repl.Mcp;
using Repl.Parameters;

namespace Guppi.Repl.Skills
{

    public sealed class HueModule(IHueLightService hue) : IReplModule
    {
        public void Map(IReplMap map)
        {
            map.Map("bridges", async (CancellationToken cancellationToken) =>
                {
                    var bridges = await hue.ListBridges().WaitAsync(cancellationToken).ConfigureAwait(false);
                    return bridges
                        .Select(bridge => new HueBridgeResult(bridge.BridgeId, bridge.IpAddress))
                        .ToArray();
                })
                .WithDescription("Discover Philips Hue bridges")
                .ReadOnly()
                .OpenWorld()
                .LongRunning();

            map.Map("lights", async (
                    [ReplOption(Aliases = ["-i"])] string ip = null,
                    CancellationToken cancellationToken = default) =>
                {
                    var lights = await hue.ListLights(ip, RejectInteractiveRegistration)
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return lights.Select(ToResult).ToArray();
                })
                .WithDescription("List Philips Hue lights")
                .ReadOnly()
                .OpenWorld()
                .LongRunning();

            map.Map("{light} on", async (
                    string light,
                    [ReplOption(Aliases = ["-i"])] string ip = null,
                    [ReplOption(Aliases = ["-b"])] byte? brightness = null,
                    [ReplOption(Aliases = ["-c"])] string color = null,
                    CancellationToken cancellationToken = default) =>
                {
                    var (target, lightId) = await ResolveLight(light, ip, cancellationToken).ConfigureAwait(false);
                    await hue.SetLight(new SetLightCommand
                    {
                        IpAddress = ip,
                        Light = lightId,
                        On = true,
                        Brightness = brightness,
                        Color = color,
                        WaitForUserInput = RejectInteractiveRegistration,
                    })
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return new HueActionResult(target.Id, target.Name, true, brightness, color, ip);
                })
                .WithDescription("Turn on a Philips Hue light by ID or name")
                .WithCompletion("light", CompleteLightsAsync)
                .OpenWorld()
                .Idempotent()
                .LongRunning();

            map.Map("{light} off", async (
                    string light,
                    [ReplOption(Aliases = ["-i"])] string ip = null,
                    CancellationToken cancellationToken = default) =>
                {
                    var (target, lightId) = await ResolveLight(light, ip, cancellationToken).ConfigureAwait(false);
                    await hue.SetLight(new SetLightCommand
                    {
                        IpAddress = ip,
                        Light = lightId,
                        Off = true,
                        WaitForUserInput = RejectInteractiveRegistration,
                    })
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return new HueActionResult(target.Id, target.Name, false, null, null, ip);
                })
                .WithDescription("Turn off a Philips Hue light by ID or name")
                .WithCompletion("light", CompleteLightsAsync)
                .OpenWorld()
                .Idempotent()
                .LongRunning();
        }

        private async ValueTask<IReadOnlyList<string>> CompleteLightsAsync(
            CompletionContext _,
            string input,
            CancellationToken cancellationToken)
        {
            var lights = await hue.ListLights(null, RejectInteractiveRegistration)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            return lights
                .SelectMany(light => new[] { light.Name, light.Id })
                .Where(value => !string.IsNullOrWhiteSpace(value)
                    && value.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private async Task<(HueLight Light, uint Id)> ResolveLight(
            string light,
            string ip,
            CancellationToken cancellationToken)
        {
            var lights = await hue.ListLights(ip, RejectInteractiveRegistration)
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            var target = lights.FirstOrDefault(candidate =>
                string.Equals(candidate.Id, light, StringComparison.OrdinalIgnoreCase)
                || string.Equals(candidate.Name, light, StringComparison.OrdinalIgnoreCase));
            if (target is null)
            {
                throw new InvalidOperationException($"Hue light {light} was not found.");
            }

            if (!uint.TryParse(target.Id, out var lightId))
            {
                throw new InvalidOperationException($"Hue light {target.Name} has invalid ID {target.Id}.");
            }

            return (target, lightId);
        }

        private static HueLightResult ToResult(Guppi.Core.Entities.Hue.HueLight light) =>
            new(light.Id, light.Name, light.On, light.Brightness, light.Color, light.Type);

        private static void RejectInteractiveRegistration(string message) =>
            throw new InvalidOperationException(
                $"{message}. Register first with: guppi hue register --ip <address>.");
    }
}
