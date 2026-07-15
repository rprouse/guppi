using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Services.Hue;

namespace Guppi.Core.Interfaces.Services;

public interface IHueLightService
{
    void Configure();
    Task<bool> Register(string ipAddress, Action<string> waitForUserInput, CancellationToken cancellationToken = default);
    uint GetDefaultLight();
    Task<IEnumerable<HueLight>> ListLights(string ipAddress, Action<string> waitForUserInput, CancellationToken cancellationToken = default);
    Task<IEnumerable<HueBridge>> ListBridges(CancellationToken cancellationToken = default);
    Task SetLight(SetLightCommand request, CancellationToken cancellationToken = default);
}
