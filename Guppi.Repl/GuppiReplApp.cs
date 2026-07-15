using System;
using Guppi.Core;
using Guppi.Repl.Skills;
using Microsoft.Extensions.DependencyInjection;
using Repl;
using Repl.Spectre;
using Repl.Terminal;

namespace Guppi.Repl;

public static class GuppiReplApp
{
    public static ReplApp Create(Action<IServiceCollection> configureServices = null)
    {
        var app = ReplApp.Create(services =>
            {
                services.AddCore();
                services.AddTransient<ILocalIpAddressProvider, LocalIpAddressProvider>();
                services.AddSingleton(TimeProvider.System);
                services.AddSpectreConsole();
                configureServices?.Invoke(services);
            })
            .WithDescription("Experimental Guppi command graph shared by CLI, REPL, and MCP.")
            .UseTerminalIntegration()
            .UseDefaultInteractive()
            .UseCliProfile()
            .UseSpectreConsole();

        app.Context("utilities", utilities => utilities.MapModule<UtilitiesModule>());
        app.Context("ip", ip => ip.MapModule<IpModule>());

        return app;
    }
}
