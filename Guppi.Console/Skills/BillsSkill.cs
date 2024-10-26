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
        var alectra = new Command("alectra", "Download bills from Alectra")
        {
        };
        alectra.Handler = CommandHandler.Create(async () => await DownloadAlectraBills());

        var configure = new Command("configure", "Configures the Bill provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => Configure());

        var command = new Command("bills", "Download bills from online")
        {
            alectra,
            configure
        };
        command.AddAlias("billing");
        command.AddAlias("bill");

        return new List<Command> { command };
    }

    private async Task DownloadAlectraBills()
    {
        try
        {
            AnsiConsoleHelper.TitleRule(":high_voltage: Alectra Bills");

            await _service.DownloadAlectraBills();

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
