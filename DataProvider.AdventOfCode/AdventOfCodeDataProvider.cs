using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Guppi.Core;

namespace DataProvider.AdventOfCode
{
    public class AdventOfCodeDataProvider : IDataProvider
    {
        AdventOfCodeConfiguration _configuration;
        LeaderBoardService _leaderBoardService;

        public AdventOfCodeDataProvider()
        {
            _configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            _leaderBoardService = new LeaderBoardService(_configuration);
        }

        public Command GetCommand()
        {
            var view = new Command("view", "Views the Advent of Code leaderboard")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Displays the leaderboard for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--board", "-b" }, () => 274125, "The id of the leaderboard you want to view. Defaults to the CoderCamp leaderboard")
            };

            view.Handler = CommandHandler.Create(async (int year, int board) => await _leaderBoardService.ViewLeaderboard(year, board));

            var configure = new Command("configure", "Configures the AdventOfCode provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(() => Configure());

            return new Command("aoc", "Work with Advent of Code (AoC)")
            {
               view,
               configure
            };
        }

        private void Configure()
        {
            _configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
        }
    }
}
