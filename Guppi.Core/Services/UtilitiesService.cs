using System.Threading.Tasks;
using Guppi.Domain.Interfaces;

namespace Guppi.Core.Services;

internal sealed class UtilitiesService : IUtilitiesService
{
    const string Explorer = "explorer";

    readonly IProcessService _process;

    public UtilitiesService(IProcessService process)
    {
        _process = process;
    }

    public async Task RestartExplorer()
    {
        _process.Kill(Explorer);
        // Wait to give Explorer a chance to restart itself
        await Task.Delay(500);
        if (!_process.Running(Explorer))
            _process.Start(Explorer);
    }
}
