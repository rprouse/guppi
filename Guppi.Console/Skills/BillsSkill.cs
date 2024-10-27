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
        var option = new Option<int>(new string[] { "--months", "-m" }, () => 1, "The number of months of bills to download.");

        var all = new Command("all", "Download bills from all providers")
        {
            option
        };
        all.Handler = CommandHandler.Create(async (int months) => await DownloadAllBills(months));

        var alectra = new Command("alectra", "Download bills from Alectra")
        {
            option
        };
        alectra.Handler = CommandHandler.Create(async (int months) => await DownloadAlectraBills(months));

        var enbridge = new Command("enbridge", "Download bills from Enbridge")
        {
            option
        };
        enbridge.Handler = CommandHandler.Create(async (int months) => await DownloadEnbridgeBills(months));

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

    private async Task DownloadAllBills(int months) =>
        await DownloadBills(":spiral_notepad: Download Bills", months, _service.DownloadAllBills);

    private async Task DownloadAlectraBills(int months) =>
        await DownloadBills(":high_voltage: Alectra Bills", months, _service.DownloadAlectraBills);

    private async Task DownloadEnbridgeBills(int months) =>
        await DownloadBills(":chart_increasing: Enbridge Bills", months, _service.DownloadEnbridgeBills);

    private static async Task DownloadBills(string title, int months, Func<int, Task> downloader)
    { 
        try
        {
            AnsiConsoleHelper.TitleRule(title);

            await downloader(months);

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
