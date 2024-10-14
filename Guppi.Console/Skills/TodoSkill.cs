using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Services;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class TodoSkill(ITodoService service) : ISkill
{
    private readonly ITodoService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var sync = new Command("sync", "Syncs the todo list with Google Tasks.")
        {
            Handler = CommandHandler.Create(async () => await Sync())
        };

        var todo = new Command("todo", "Works with the todo list")
        {
            sync
        };
        return [todo];
    }

    private async Task Sync()
    {
        try
        {
            await _service.Sync();
        }
        catch (ErrorException ee)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ee.Message}]][/]");
        }
        catch (UnauthorizedException ue)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: ${ue.Message}]][/]");
        }
    }
}
