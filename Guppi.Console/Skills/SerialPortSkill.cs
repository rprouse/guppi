using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using Guppi.Application.Extensions;
using Guppi.Application.Services;
using Spectre.Console;

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

        var hex = new Command("hex", "Uploads HEX files to the Agon Light computer over serial")
        {
            new Argument<FileInfo>("filename", "The HEX file to upload"),
            new Argument<string>("port", () => "COM4", "The serial port to upload to"),
            new Argument<int>("baud", () => 115200, "The baud rate to use")
        };
        hex.Handler = CommandHandler.Create<FileInfo, string, int>(UploadHex);

        return new[] { ports, hex };
    }

    private void ListPorts()
    {
        AnsiConsoleHelper.TitleRule(":electric_plug: Serial Ports");
        var ports = _service.GetPorts();
        foreach (var port in ports.OrderBy(p => p))
        {
            System.Console.WriteLine(port);
        }
    }

    private void UploadHex(FileInfo filename, string port, int baud)
    {
        AnsiConsoleHelper.TitleRule(":satellite_antenna: Upload HEX File to Agon Light");

        AnsiConsole.MarkupLine($"[cyan]File:[/] {filename.Name}");
        AnsiConsole.MarkupLine($"[cyan]Port:[/] {port}");
        AnsiConsole.MarkupLine($"[cyan]Baud:[/] {baud}");
        AnsiConsole.WriteLine();

        if (!filename.Exists) 
        { 
            AnsiConsole.MarkupLine($"[yellow]:yellow_circle: File does not exist[/]");
            return;
        }

        if (!_service.GetPorts().Contains(port))
        {
            AnsiConsole.MarkupLine($"[yellow]:yellow_circle: Port does not exist[/]");
            return;
        }

        AnsiConsole.MarkupLine("[green]:green_circle: Uploading...[/]");
        try
        {
            _service.UploadHex(port, baud, filename.FullName, (s) => AnsiConsole.MarkupLine(s));
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]:red_circle: {ex.Message}[/]");
        }
    }
}
