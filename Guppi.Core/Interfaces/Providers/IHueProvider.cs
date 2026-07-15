using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;

namespace Guppi.Core.Interfaces.Providers
{
    public interface IHueProvider
    {
        Task<IEnumerable<HueBridge>> ListBridges(CancellationToken cancellationToken = default);

        Task<IEnumerable<HueLight>> ListLights(string ip, CancellationToken cancellationToken = default);

        Task Set(string ip, bool on, bool off, bool alert, byte? brightness, string color, uint light, CancellationToken cancellationToken = default);

        Task<bool> ConnectToBridge(string ip = null, bool loadKey = true, CancellationToken cancellationToken = default);

        Task<bool> Register(string ip = null, CancellationToken cancellationToken = default);

        Action<string> WaitForUserInput { get; set; }
    }
}
