using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Services.Hue;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Interfaces.Providers;

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

    public Task<IEnumerable<HueBridge>> ListBridges() =>
        _hueService.ListBridges();

    public async Task<IEnumerable<HueLight>> ListLights(string ipAddress, Action<string> waitForUserInput)
    {
        _hueService.WaitForUserInput = waitForUserInput;
        return await _hueService.ListLights(ipAddress);
    }

    public async Task<bool> Register(string ipAddress, Action<string> waitForUserInput)
    {
        _hueService.WaitForUserInput = waitForUserInput;
        return await _hueService.Register(ipAddress);
    }

    public async Task SetLight(SetLightCommand request)
    {
        _hueService.WaitForUserInput = request.WaitForUserInput;
        await _hueService.Set(request.IpAddress, request.On, request.Off, request.Alert, request.Brightness, request.Color, request.Light);
    }
}
