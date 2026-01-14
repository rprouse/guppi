using System;
using Guppi.Core.Entities.Strava;

namespace Guppi.Core.Services.Strava
{
#pragma warning disable IDE1006 // Naming Styles
    public class StravaActivity
    {
        public Athlete athlete { get; set; }
        public string name { get; set; }
        public double? distance { get; set; }
        public int? moving_time { get; set; }
        public int? elapsed_time { get; set; }
        public double? total_elevation_gain { get; set; }
        public string type { get; set; }
        public long id { get; set; }
        public string external_id { get; set; }
        public long? upload_id { get; set; }
        public DateTimeOffset start_date { get; set; }
        public DateTimeOffset start_date_local { get; set; }
        public string timezone { get; set; }
        public double utc_offset { get; set; }
        public string location_country { get; set; }
        public int? achievement_count { get; set; }
        public int? kudos_count { get; set; }
        public int? comment_count { get; set; }
        public int? athlete_count { get; set; }
        public int? photo_count { get; set; }
        public bool trainer { get; set; }
        public bool commute { get; set; }
        public bool manual { get; set; }
        public bool _private { get; set; }
        public bool flagged { get; set; }
        public string gear_id { get; set; }
        public float? average_speed { get; set; }
        public float? max_speed { get; set; }
        public float? average_cadence { get; set; }
        public float? average_watts { get; set; }
        public int? weighted_average_watts { get; set; }
        public float? kilojoules { get; set; }
        public bool device_watts { get; set; }
        public bool has_heartrate { get; set; }
        public double? average_heartrate { get; set; }
        public double? max_heartrate { get; set; }
        public double? max_watts { get; set; }
        public int? pr_count { get; set; }
        public int? total_photo_count { get; set; }
        public bool has_kudoed { get; set; }
        public double? suffer_score { get; set; }

        public Activity GetActivity() =>
            new Activity
            {
                Id = id,
                Name = name,
                Type = type,
                Distance = distance ?? 0,
                MaxSpeed = max_speed ?? 0,
                MovingTime = TimeSpan.FromSeconds(moving_time ?? 0),
                ElapsedTime = TimeSpan.FromSeconds(elapsed_time ?? 0),
                Elevation = total_elevation_gain ?? 0,
                StartDate = start_date,
                Kudos = kudos_count ?? 0,
                SufferScore = suffer_score ?? 0,
                Icon = FitnessIcon.Icons[type]
            };
    }

    public class Athlete
    {
        public long id { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}
