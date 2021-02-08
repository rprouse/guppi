using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Hue;

namespace Guppi.Domain.Interfaces
{
    public interface IHueService
    {
        Task<IEnumerable<HueBridge>> ListBridges();

        Task<IEnumerable<HueLight>> ListLights(string ip);

        Task Set(string ip, bool on, bool off, bool alert, byte? brightness, string color, uint light);

        Task<bool> ConnectToBridge(string ip = null, bool loadKey = true);

        Task<bool> Register(string ip = null);

        Action<string> WaitForUserInput { get; set; }

        void Configure();

        uint GetDefaultLight();
    }
}
