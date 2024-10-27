using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces.Services;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class BillsSkill(IBillService service) : ISkill
{
    private readonly IBillService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var all = new Command("all", "Download bills from all providers")
        {
        };
        all.Handler = CommandHandler.Create(async () => await DownloadAllBills());

        var alectra = new Command("alectra", "Download bills from Alectra")
        {
        };
        alectra.Handler = CommandHandler.Create(async () => await DownloadAlectraBills());

        var enbridge = new Command("enbridge", "Download bills from Enbridge")
        {
        };
        enbridge.Handler = CommandHandler.Create(async () => await DownloadEnbridgeBills());

        var configure = new Command("configure", "Configures the Bill provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => Configure());

        var command = new Command("bills", "Download bills from online")
        {
            all,
            alectra,
            enbridge,
            configure
        };
        command.AddAlias("billing");
        command.AddAlias("bill");

        return new List<Command> { command };
    }

    private async Task DownloadAllBills() =>
        await DownloadBills(":spiral_notepad: Download Bills", _service.DownloadAllBills);

    private async Task DownloadAlectraBills() =>
        await DownloadBills(":high_voltage: Alectra Bills", _service.DownloadAlectraBills);

    private async Task DownloadEnbridgeBills() =>
        await DownloadBills(":chart_increasing: Enbridge Bills", _service.DownloadEnbridgeBills);

    private static async Task DownloadBills(string title, Func<Task> downloader)
    { 
        try
        {
            AnsiConsoleHelper.TitleRule(title);

            await downloader();

            AnsiConsoleHelper.Rule("white");
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
        catch (UnauthorizedException ue)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
        }
    }

    private void Configure() => _service.Configure();
}
