using System;
using System.Collections.Generic;

namespace Guppi.Domain.Entities.AdventOfCode
{
    public class Leaderboard
    {
        public string Year { get; init; }
        public int OwnerId { get; init; }

        public Dictionary<string, Member> Members { get; init; }
    }

    public class Member
    {
        public string Name { get; init; }
        public int Id { get; init; }
        public int GlobalScore { get; init; }
        public int LocalScore { get; init; }
        public int Stars { get; init; }
        public Dictionary<string, Day> CompletionDayLevel { get; init; }
    }

    public class Day
    {
        public bool PartOneComplete { get; init; }
        public bool PartTwoComplete { get; init; }
    }
}
