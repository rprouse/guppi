﻿using System.Threading.Tasks;
using Guppi.Core.Interfaces.Providers;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

internal sealed class UtilitiesService : IUtilitiesService
{
    const string Explorer = "explorer";

    readonly IProcessProvider _process;

    public UtilitiesService(IProcessProvider process)
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
