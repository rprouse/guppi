using System;
using Guppi.Domain.Entities.Strava;

namespace Guppi.Infrastructure.Services.Strava
{
#pragma warning disable IDE1006 // Naming Styles
    public class Activity
    {
        public int resource_state { get; set; }
        public Athlete athlete { get; set; }
        public string name { get; set; }
        public double distance { get; set; }
        public int moving_time { get; set; }
        public int elapsed_time { get; set; }
        public double total_elevation_gain { get; set; }
        public string type { get; set; }
        public object workout_type { get; set; }
        public long id { get; set; }
        public string external_id { get; set; }
        public long upload_id { get; set; }
        public DateTimeOffset start_date { get; set; }
        public DateTimeOffset start_date_local { get; set; }
        public string timezone { get; set; }
        public double utc_offset { get; set; }
        public object start_latlng { get; set; }
        public object end_latlng { get; set; }
        public object location_city { get; set; }
        public object location_state { get; set; }
        public string location_country { get; set; }
        public int achievement_count { get; set; }
        public int kudos_count { get; set; }
        public int comment_count { get; set; }
        public int athlete_count { get; set; }
        public int photo_count { get; set; }
        public Map map { get; set; }
        public bool trainer { get; set; }
        public bool commute { get; set; }
        public bool manual { get; set; }
        public bool _private { get; set; }
        public bool flagged { get; set; }
        public string gear_id { get; set; }
        public bool from_accepted_tag { get; set; }
        public float average_speed { get; set; }
        public float max_speed { get; set; }
        public float average_cadence { get; set; }
        public float average_watts { get; set; }
        public int weighted_average_watts { get; set; }
        public float kilojoules { get; set; }
        public bool device_watts { get; set; }
        public bool has_heartrate { get; set; }
        public double average_heartrate { get; set; }
        public double max_heartrate { get; set; }
        public double max_watts { get; set; }
        public int pr_count { get; set; }
        public int total_photo_count { get; set; }
        public bool has_kudoed { get; set; }
        public double? suffer_score { get; set; }

        public StravaActivity GetActivity() =>
            new StravaActivity
            {
                Id = id,
                Name = name,
                Type = type,
                Distance = distance,
                MovingTime = TimeSpan.FromSeconds(moving_time),
                ElapsedTime = TimeSpan.FromSeconds(elapsed_time),
                Elevation = total_elevation_gain,
                StartDate = start_date,
                Kudos = kudos_count,
                SufferScore = suffer_score,
                Icon = FitnessIcon.Icons[type]
            };
    }

    public class Athlete
    {
        public long id { get; set; }
        public int resource_state { get; set; }
    }

    public class Map
    {
        public string id { get; set; }
        public object summary_polyline { get; set; }
        public int resource_state { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}
