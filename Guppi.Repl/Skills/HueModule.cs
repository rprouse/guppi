using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Services.Hue;
using Guppi.Repl.Results;
using Microsoft.Extensions.DependencyInjection;
using Repl;
using Repl.Mcp;
using Repl.Parameters;

namespace Guppi.Repl.Skills
{

    public sealed class HueModule(IServiceScopeFactory scopeFactory) : IReplModule
    {
        public void Map(IReplMap map)
        {
            map.Map("bridges", async (CancellationToken cancellationToken) =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var hue = scope.ServiceProvider.GetRequiredService<IHueLightService>();
                    var bridges = await hue.ListBridges(cancellationToken).ConfigureAwait(false);
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
                    using var scope = scopeFactory.CreateScope();
                    var hue = scope.ServiceProvider.GetRequiredService<IHueLightService>();
                    var lights = await hue.ListLights(ip, RejectInteractiveRegistration, cancellationToken)
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
                    [Description("Brightness percentage from 0 to 100.")]
                    [ReplOption(Aliases = ["-b"])] int brightness = -1,
                    [ReplOption(Aliases = ["-c"])] string color = null,
                    CancellationToken cancellationToken = default) =>
                {
                    var normalizedBrightness = NormalizeBrightness(brightness);
                    using var scope = scopeFactory.CreateScope();
                    var hue = scope.ServiceProvider.GetRequiredService<IHueLightService>();
                    var (target, lightId) = await ResolveLight(hue, light, ip, cancellationToken).ConfigureAwait(false);
                    await hue.SetLight(new SetLightCommand
                    {
                        IpAddress = ip,
                        Light = lightId,
                        On = true,
                        Brightness = normalizedBrightness,
                        Color = color,
                        WaitForUserInput = RejectInteractiveRegistration,
                    }, cancellationToken)
                        .ConfigureAwait(false);
                    return new HueActionResult(target.Id, target.Name, true, normalizedBrightness, color, ip);
                })
                .WithDescription("Turn on a Philips Hue light by ID or name")
                .WithCompletion("light", CompleteLightsAsync)
                .Destructive()
                .OpenWorld()
                .Idempotent()
                .LongRunning();

            map.Map("{light} off", async (
                    string light,
                    [ReplOption(Aliases = ["-i"])] string ip = null,
                    CancellationToken cancellationToken = default) =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var hue = scope.ServiceProvider.GetRequiredService<IHueLightService>();
                    var (target, lightId) = await ResolveLight(hue, light, ip, cancellationToken).ConfigureAwait(false);
                    await hue.SetLight(new SetLightCommand
                    {
                        IpAddress = ip,
                        Light = lightId,
                        Off = true,
                        WaitForUserInput = RejectInteractiveRegistration,
                    }, cancellationToken)
                        .ConfigureAwait(false);
                    return new HueActionResult(target.Id, target.Name, false, null, null, ip);
                })
                .WithDescription("Turn off a Philips Hue light by ID or name")
                .WithCompletion("light", CompleteLightsAsync)
                .Destructive()
                .OpenWorld()
                .Idempotent()
                .LongRunning();
        }

        private async ValueTask<IReadOnlyList<string>> CompleteLightsAsync(
            CompletionContext _,
            string input,
            CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var hue = scope.ServiceProvider.GetRequiredService<IHueLightService>();
            var lights = await hue.ListLights(null, RejectInteractiveRegistration, cancellationToken)
                .ConfigureAwait(false);
            return lights
                .SelectMany(light => new[] { light.Name, light.Id })
                .Where(value => !string.IsNullOrWhiteSpace(value)
                    && value.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static async Task<(HueLight Light, uint Id)> ResolveLight(
            IHueLightService hue,
            string light,
            string ip,
            CancellationToken cancellationToken)
        {
            var lights = await hue.ListLights(ip, RejectInteractiveRegistration, cancellationToken)
                .ConfigureAwait(false);
            HueLight target;
            if (uint.TryParse(light, out _))
            {
                target = lights.SingleOrDefault(candidate =>
                    string.Equals(candidate.Id, light, StringComparison.Ordinal));
            }
            else
            {
                var matches = lights
                    .Where(candidate => string.Equals(
                        candidate.Name,
                        light,
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (matches.Length > 1)
                {
                    var candidateIds = string.Join(", ", matches
                        .Select(candidate => candidate.Id)
                        .OrderBy(id => id, StringComparer.Ordinal));
                    throw new InvalidOperationException(
                        $"Hue light name {light} is ambiguous. Use one of these IDs: {candidateIds}.");
                }

                target = matches.SingleOrDefault();
            }

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

        private static byte? NormalizeBrightness(int brightness)
        {
            if (brightness == -1)
            {
                return null;
            }

            if (brightness is < 0 or > 100)
            {
                throw new InvalidOperationException("Brightness must be between 0 and 100 percent.");
            }

            return (byte)brightness;
        }

        private static HueLightResult ToResult(HueLight light) =>
            new(light.Id, light.Name, light.On, light.Brightness, light.Color, light.Type);

        private static void RejectInteractiveRegistration(string message) =>
            throw new InvalidOperationException(
                $"{message}. Register first with: guppi hue register --ip <address>.");
    }
}
