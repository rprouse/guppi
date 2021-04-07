using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Commands.AdventOfCode;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.AdventOfCode;
using Guppi.Domain.Entities.AdventOfCode;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Actions
{
    public class AdventOfCodeDataProvider : IActionProvider
    {
        private readonly IMediator _mediator;

        public AdventOfCodeDataProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Command GetCommand()
        {
            var add = new Command("add", "Adds a new day to my Advent of Code solution")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Adds a day to the given year. Defaults to this year."),
            };
            add.Handler = CommandHandler.Create(async (int year) => await AddDayTo(year));

            var test = new Command("test", "Runs tests in my Advent of Code solution")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Runs tests for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--day", "-d" }, () => 0, "Runs tests for the given day. If not set, runs all tests for the year."),
            };
            test.Handler = CommandHandler.Create(async (int year, int day) => await RunTests(year, day));

            var view = new Command("view", "Views the Advent of Code leaderboard")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Displays the leaderboard for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--board", "-b" }, () => 274125, "The id of the leaderboard you want to view. Defaults to the CoderCamp leaderboard")
            };
            view.Handler = CommandHandler.Create(async (int year, int board) => await ViewLeaderboard(year, board));

            var configure = new Command("configure", "Configures the AdventOfCode provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(async () => await Configure());

            var command = new Command("aoc", "Work with Advent of Code (AoC)")
            {
                add,
                test,
                view,
                configure
            };
            command.AddAlias("advent");
            return command;
        }

        private async Task ViewLeaderboard(int year, int board)
        {
            try
            {
                var leaders = await _mediator.Send(new LeaderboardQuery { Year = year, Board = board });

                AnsiConsole.WriteLine();
                AnsiConsoleHelper.TitleRule($":christmas_tree: Advent of Code Leaderboard {year}");

                int place = 1;
                foreach (var member in leaders.Members.Values.OrderByDescending(m => m.LocalScore).ThenByDescending(m => m.Stars))
                {
                    AnsiConsole.Markup(string.Format("[white]{0,3}) {1,3}[/]  ", place++, member.LocalScore));

                    for (int d = 1; d <= 25; d++)
                    {
                        if (DateTime.Now.Date < new DateTime(year, 12, d))
                        {
                            AnsiConsole.Markup("[grey]·[/]");
                            continue;
                        }

                        string day = d.ToString();
                        string star;
                        if (member.CompletionDayLevel.ContainsKey(day))
                        {
                            Day completed = member.CompletionDayLevel[day];
                            if (completed.PartOneComplete && completed.PartTwoComplete)
                                star = "[yellow]*[/]";
                            else
                                star = "[white]*[/]";
                        }
                        else
                        {
                            star = "[grey]*[/]";
                        }
                        AnsiConsole.Markup(star);
                    }
                    AnsiConsole.MarkupLine($"[green]  {member.Name}[/]");
                }
                AnsiConsoleHelper.Rule("white");
            }
            catch (UnconfiguredException ex)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ex.Message}]][/]");
            }
        }

        private async Task AddDayTo(int year)
        {
            try
            {
                var result = await _mediator.Send(new AddDayCommand { Year = year });

                AnsiConsole.MarkupLine($"[green][[:check_mark_button: Adding new day to {result.Directory}]][/]");
                AnsiConsole.MarkupLine($"[green][[:check_mark_button: Added Day{result.NewDay:00}]][/]");
            }
            catch (WarningException ex)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ex.Message}]][/]");
                AnsiConsole.MarkupLine("[silver][[No day added.]][/]");
            }
            catch (UnconfiguredException ex)
            {
                AnsiConsole.MarkupLine($"[red][[:cross_mark: {ex.Message}]][/]");
                AnsiConsole.MarkupLine("[silver][[Configure the data provider to set the solution directory.]][/]");
            }
        }

        private async Task Configure() => await _mediator.Send(new ConfigureAocCommand());

        private async Task RunTests(int year, int day)
        {
            try
            {
                await _mediator.Send(new RunTestsCommand { Year = year, Day = day });
            }
            catch (UnconfiguredException ex)
            {
                AnsiConsole.MarkupLine($"[red][[:cross_mark: {ex.Message}]][/]");
                AnsiConsole.MarkupLine("[silver][[Configure the data provider to set the solution directory.]][/]");
                return;
            }
        }
    }
}
