using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Providers;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Services.Hue;

namespace Guppi.Core.Services;

internal sealed class HueLightService : IHueLightService
{
    private readonly IHueProvider _hueService;

    public HueLightService(IHueProvider hueService)
    {
        _hueService = hueService;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<HueConfiguration>("hue");
        configuration.RunConfiguration("Hue Lights", "Enter your default light");
    }

    public uint GetDefaultLight() =>
        Configuration.Load<HueConfiguration>("hue").GetDefaultLight();

    public Task<IEnumerable<HueBridge>> ListBridges(CancellationToken cancellationToken = default) =>
        _hueService.ListBridges(cancellationToken);

    public async Task<IEnumerable<HueLight>> ListLights(
        string ipAddress,
        Action<string> waitForUserInput,
        CancellationToken cancellationToken = default)
    {
        _hueService.WaitForUserInput = waitForUserInput;
        return await _hueService.ListLights(ipAddress, cancellationToken);
    }

    public async Task<bool> Register(
        string ipAddress,
        Action<string> waitForUserInput,
        CancellationToken cancellationToken = default)
    {
        _hueService.WaitForUserInput = waitForUserInput;
        return await _hueService.Register(ipAddress, cancellationToken);
    }

    public async Task SetLight(SetLightCommand request, CancellationToken cancellationToken = default)
    {
        _hueService.WaitForUserInput = request.WaitForUserInput;
        await _hueService.Set(request.IpAddress, request.On, request.Off, request.Alert, request.Brightness, request.Color, request.Light, cancellationToken);
    }
}
