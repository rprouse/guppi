using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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

            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("Dec", "Hex", "HTML", "Char", "Description");
            table.AddColumns("Dec", "Hex", "HTML", "Char", "Description");

            for (int i=0; i < 64; i++)
            {
                table.AddRow(
                    data[i].Value.ToString(),
                    $"0x{data[i].Value.ToString("X2")}",
                    $"&#{data[i].Value};",
                    data[i].Character.EscapeMarkup(),
                    data[i].Description,

                    data[i+64].Value.ToString(),
                    $"0x{data[i+64].Value.ToString("X2")}",
                    $"&#{data[i+64].Value};",
                    data[i+64].Character.EscapeMarkup(),
                    data[i+64].Description
                    );
            }

            AnsiConsole.Render(table);
            AnsiConsoleHelper.Rule("white");
        }
    }
}
