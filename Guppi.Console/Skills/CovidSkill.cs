using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.Covid;
using Guppi.Domain.Common;
using Guppi.Domain.Entities.Covid;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    public class CovidSkill : ISkill
    {
        private readonly IMediator _mediator;

        public CovidSkill(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEnumerable<Command> GetCommands()
        {
            var view = new Command("view", "Views Covid-19 Stats for a country. Defaults to Canada.")
            {
                new Option<string>(new string[]{"--country" }, () => "Canada", "The country to view. Defaults to Canada. Valid countries, Canada, Brazil, Germany, Spain, France, UK, India, Italy, Mexico and USA"),
                new Option<bool>(new string[]{"--cases", "-c"}, () => false, "Views the cases for provinces or states."),
                new Option<bool>(new string[]{"--deaths", "-d"}, () => false, "View deaths for provinces or states.")
            };
            view.Handler = CommandHandler.Create(async (string country, bool cases, bool deaths) => await View(country, cases, deaths));

            return new[] {
                new Command("covid", "Displays Covid-19 Stats") { view }
            };
        }

        private async Task View(string country, bool cases, bool deaths)
        {
            try
            {
                var c = country.GetCountry();
                if (c == Country.Unknown)
                {
                    AnsiConsole.MarkupLine($"[red][[:cross_mark: Could not find country ${country}]][/]");
                    return;
                }

                CovidData data = await _mediator.Send(new CovidCountryDataQuery { Country = c });

                AnsiConsoleHelper.TitleRule($":biohazard: Covid data for {country}");

                DisplayCountryData(data);
                AnsiConsole.WriteLine();

                if (cases)
                {
                    DisplayRegionalCases(data);
                    AnsiConsole.WriteLine();
                }

                if (deaths)
                {
                    DisplayRegionalDeaths(data);
                    AnsiConsole.WriteLine();
                }

                AnsiConsoleHelper.Rule("white");
            }
            catch (Exception ue)
            {
                AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
            }
        }

        private static void DisplayCountryData(CovidData data)
        {
            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("", "Total Reported", "Per 100k", data.RegionData.LatestDate.ToString("yyyy-MM-dd"), "Weekly Trend");
            table.Columns[0].LeftAligned();
            table.Columns[1].RightAligned();
            table.Columns[2].RightAligned();
            table.Columns[3].RightAligned();
            table.Columns[4].RightAligned();
            string casesColor = data.RegionData.CasesWeeklyTrend > 0 ? "[red]↗ " : "[green]↘ ";
            table.AddRow(
                "[white]Cases[/]",
                data.RegionData.LatestCases.ToString("n0"),
                data.RegionData.CasesPerHundredThousand.ToString("n0"),
                data.RegionData.LastReportedCases.ToString("n0"),
                casesColor + data.RegionData.CasesWeeklyTrend + "%[/]"
                );

            string deathsColor = data.RegionData.DeathsWeeklyTrend > 0 ? "[red]↗ " : "[green]↘ ";
            table.AddRow(
                "[white]Deaths[/]",
                data.RegionData.LatestDeaths.ToString("n0"),
                data.RegionData.DeathsPerHundredThousand.ToString("n0"),
                data.RegionData.LastReportedDeaths.ToString("n0"),
                deathsColor + data.RegionData.DeathsWeeklyTrend + "%[/]"
                );
            AnsiConsole.Render(table);
        }

        private static void DisplayRegionalCases(CovidData data)
        {
            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("", "Cases", "Per 100k", "7d Avg", "Per 100k");
            table.Columns[0].LeftAligned();
            table.Columns[1].RightAligned();
            table.Columns[2].RightAligned();
            table.Columns[3].RightAligned();
            table.Columns[4].RightAligned();

            var subregions = data.SubRegionData
                .Where(d => d.Name != "Unknown")
                .OrderByDescending(d => d.DailyAverageCasesLastSevenDaysPerHundredThousand);

            foreach (var region in subregions)
            {
                table.AddRow(
                    region.Name,
                    region.LatestCases.ToString("n0"),
                    region.CasesPerHundredThousand.ToString("n0"),
                    region.DailyAverageCasesLastSevenDays.ToString("n0"),
                    region.DailyAverageCasesLastSevenDaysPerHundredThousand.ToString("n1"));
            }
            AnsiConsole.Render(table);
        }

        private static void DisplayRegionalDeaths(CovidData data)
        {
            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("", "Deaths", "Per 100k", "7d Avg", "Per 100k");
            table.Columns[0].LeftAligned();
            table.Columns[1].RightAligned();
            table.Columns[2].RightAligned();
            table.Columns[3].RightAligned();
            table.Columns[4].RightAligned();

            var subregions = data.SubRegionData
                .Where(d => d.Name != "Unknown")
                .OrderByDescending(d => d.DailyAverageCasesLastSevenDaysPerHundredThousand);

            foreach (var region in subregions)
            {
                table.AddRow(
                    region.Name,
                    region.LatestDeaths.ToString("n0"),
                    region.DeathsPerHundredThousand.ToString("n0"),
                    region.DailyAverageDeathsLastSevenDays.ToString("n0"),
                    region.DailyAverageDeathsLastSevenDaysPerHundredThousand.ToString("n1"));
            }
            AnsiConsole.Render(table);
        }
    }
}
