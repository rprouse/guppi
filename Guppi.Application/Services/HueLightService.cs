using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Services.Hue;
using Guppi.Domain.Entities.Hue;
using Guppi.Domain.Interfaces;

namespace Guppi.Application.Services;

internal sealed class HueLightService : IHueLightService
{
    private readonly Guppi.Domain.Interfaces.IHueService _hueService;

    public HueLightService(IHueService hueService)
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
