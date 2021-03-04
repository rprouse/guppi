using System;

namespace Guppi.Domain.Entities.Strava
{
    public record StravaActivity
    {
        public long Id { get; }
        public string Name { get; }
        public string Type { get; }
        public double Distance { get; }
        public TimeSpan MovingTime { get; }
        public TimeSpan ElapsedTime { get; }
        public int Elevation { get; }
        public DateTime StartDate { get; }
        public int Kudos { get; }
        public int SufferScore { get; }
    }
}
