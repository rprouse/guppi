using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Guppi.Core;

namespace ActionProvider.AdventOfCode
{
    public class AdventOfCodeDataProvider : IActionProvider
    {
        AdventOfCodeConfiguration _configuration;
        LeaderBoardService _leaderBoardService;
        SolutionService _solutionService;
        TestService _testService;

        public AdventOfCodeDataProvider()
        {
            _configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            _leaderBoardService = new LeaderBoardService(_configuration);
            _solutionService = new SolutionService(_configuration);
            _testService = new TestService(_configuration);
        }

        public Command GetCommand()
        {
            var add = new Command("add", "Adds a new day to my Advent of Code solution")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Adds a day to the given year. Defaults to this year."),
            };
            add.Handler = CommandHandler.Create((int year) => _solutionService.AddDayTo(year));

            var test = new Command("test", "Runs tests in my Advent of Code solution")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Runs tests for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--day", "-d" }, () => 0, "Runs tests for the given day. If not set, runs all tests for the year."),
            };
            test.Handler = CommandHandler.Create((int year, int day) => _testService.RunTests(year, day));

            var view = new Command("view", "Views the Advent of Code leaderboard")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Displays the leaderboard for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--board", "-b" }, () => 274125, "The id of the leaderboard you want to view. Defaults to the CoderCamp leaderboard")
            };
            view.Handler = CommandHandler.Create(async (int year, int board) => await _leaderBoardService.ViewLeaderboard(year, board));

            var configure = new Command("configure", "Configures the AdventOfCode provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(() => Configure());

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

        private void Configure()
        {
            _configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
        }
    }
}
