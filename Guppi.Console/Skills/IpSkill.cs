using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Spectre.Console;
using Guppi.Core.Extensions;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using Guppi.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Guppi.Console.Skills;

internal class IpSkill(IIPService service) : ISkill
{
    private readonly IIPService _service = service;

    public IEnumerable<Command> GetCommands() => [
        new Command("ip", "Displays the IP address of the current machine.")
        {
            Handler = CommandHandler.Create(async () => await DisplayIp())
        }
    ];

    private async Task DisplayIp()
    {
        AnsiConsoleHelper.TitleRule($":tokyo_tower: IP Addresses");

        try
        {
            var wanIp = await _service.GetWanIPAddress();
            var interfaces = _service.GetNetworkInterfaces();

            AnsiConsole.MarkupLine("[bold]WAN IP:[/]");
            AnsiConsole.MarkupLine($"  [cyan]{wanIp}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Local IPs:[/]");

            foreach (NetworkInterface networkInterface in interfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties properties = networkInterface.GetIPProperties();

                    foreach (var ip in properties.UnicastAddresses.Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork).Select(i => i.Address))
                    {
                        AnsiConsole.MarkupLine($"  [cyan]{ip}[/]: [green]{networkInterface.Name}[/]");
                    }
                }
            }
            AnsiConsole.WriteLine();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine($"[red]{ex.Message}[/]");
        }

        AnsiConsoleHelper.Rule("white");
    }
}
