using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Guppi.Application.Extensions;
using Guppi.Application.Services;

namespace Guppi.Console.Skills;

internal class SerialPortSkill : ISkill
{
    ISerialPortService _service;

    public SerialPortSkill(ISerialPortService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var ports = new Command("ports", "Lists all available serial ports");
        ports.Handler = CommandHandler.Create(() => ListPorts());

        return new[] { ports };
    }

    private void ListPorts()
    {
        AnsiConsoleHelper.TitleRule(":electric_plug: Serial Ports");
        var ports = _service.GetPorts();
        foreach (var port in ports)
        {
            System.Console.WriteLine(port);
        }
    }
}
