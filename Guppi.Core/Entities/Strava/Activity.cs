using System;

namespace Guppi.Core.Entities.Strava
{
    public record Activity
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public double Distance { get; init; }
        public TimeSpan MovingTime { get; init; }
        public TimeSpan ElapsedTime { get; init; }
        public double Elevation { get; init; }
        public DateTimeOffset StartDate { get; init; }
        public int Kudos { get; init; }
        public double? SufferScore { get; init; }
        public string Icon { get; set; }
    }
}
