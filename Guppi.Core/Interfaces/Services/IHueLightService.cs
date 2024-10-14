using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Core.Services.Hue;
using Guppi.Core.Entities.Hue;

namespace Guppi.Core.Interfaces.Services;

public interface IHueLightService
{
    void Configure();
    Task<bool> Register(string ipAddress, Action<string> waitForUserInput);
    uint GetDefaultLight();
    Task<IEnumerable<HueLight>> ListLights(string ipAddress, Action<string> waitForUserInput);
    Task<IEnumerable<HueBridge>> ListBridges();
    Task SetLight(SetLightCommand request);
}
