using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.Ascii;
using Guppi.Domain.Entities.Ascii;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    public class AsciiSkill : ISkill
    {
        private readonly IMediator _mediator;

        public AsciiSkill(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEnumerable<Command> GetCommands() =>
            new[]
            {
                new Command("ascii", "Views an ASCII chart.")
                {
                    Handler = CommandHandler.Create(async() => await ViewAsciiTable())
                }
            };

        private async Task ViewAsciiTable()
        {
            AsciiData[] data = await _mediator.Send(new AsciiQuery());

            AnsiConsoleHelper.TitleRule(":keyboard: ASCII Table");

            DisplayCodes(data, 32, 0);

            AnsiConsoleHelper.PressEnterToContinue();

            DisplayCodes(data, 32, 64);

            AnsiConsoleHelper.Rule("white");
        }

        private static void DisplayCodes(AsciiData[] data, int num, int offset)
        {
            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("Dec", "Hex", "HTML", "Char", "Description");
            table.AddColumns("Dec", "Hex", "HTML", "Char", "Description");
            table.Columns[0].RightAligned();
            table.Columns[2].RightAligned();
            table.Columns[3].Centered();
            table.Columns[5].RightAligned();
            table.Columns[7].RightAligned();
            table.Columns[8].Centered();

            for (int i = offset; i < offset + num; i++)
            {
                table.AddRow(
                    $"[cyan]{data[i].Value}[/]",
                    $"[green]0x{data[i].Value.ToString("X2")}[/]",
                    $"[yellow]&#{data[i].Value};[/]",
                    $"[white] {data[i].Character.EscapeMarkup()}[/]",
                    data[i].Description,

                    $"[cyan]{data[i + num].Value}[/]",
                    $"[green]0x{data[i + num].Value.ToString("X2")}[/]",
                    $"[yellow]&#{data[i + num].Value};[/]",
                    $"[white] {data[i + num].Character.EscapeMarkup()}[/]",
                    data[i + num].Description
                    );
            }
            AnsiConsole.Write(table);
        }
    }
}
