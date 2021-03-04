using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.Strava;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Strava
{
    public class StravaService : IStravaService
    {
        const string Command = "strava";
        const string Name = "strava";

        StravaConfiguration _configuration;
        HttpClient _client;

        public StravaService()
        {
            _configuration = Configuration.Load<StravaConfiguration>(Command);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
        }

        public void Configure()
        {
            _configuration.RunConfiguration(Name, "Enter the Strava Client Id and Secret");
        }

        public async Task<IEnumerable<Domain.Entities.Strava.StravaActivity>> GetActivities()
        {
            if (!Configured)
            {
                throw new UnconfiguredException("Please configure the Strava provider");
            }
            string access_token = string.Empty;
            if (string.IsNullOrWhiteSpace(_configuration.RefreshToken))
            {
                string code = await Authorize();
                access_token = await GetAccessToken(code, null, "authorization_code");
            }
            else
            {
                access_token = await GetAccessToken(null, _configuration.RefreshToken, "refresh_token");
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");

            TimeSpan t = DateTime.UtcNow.AddDays(-15) - new DateTime(1970, 1, 1);
            int epoch = (int)t.TotalSeconds;
            string json = await _client.GetStringAsync($"https://www.strava.com/api/v3/athlete/activities?after={epoch}&per_page=50");
            var activities = JsonSerializer.Deserialize<List<Activity>>(json);
            return activities.Select(a => a.GetActivity());
        }

        private async Task<string> Authorize()
        {
            int port = 39428;
            string url = $"http://www.strava.com/oauth/authorize?client_id={_configuration.ClientId}&response_type=code&redirect_uri=http://localhost:{port}&approval_prompt=auto&scope=read,activity:read_all";
            OpenUrl(url);

            using var listener = new HttpListener();

            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerResponse response = context.Response;

            bool result;
            string code = context.Request.QueryString["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
                result = false;
                byte[] errorBytes = Encoding.UTF8.GetBytes("<html><body><h2>Failed to log in to Strava</h2><h3>You may close this window</h3></body></html>");
                response.Headers.Clear();
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                response.Close();
                await Task.Delay(100);
                listener.Stop();
                throw new UnauthorizedException("Failed to log into Strava");
            }

            _configuration.RefreshToken = code;
            _configuration.Save();

            result = true;
            byte[] bytes = Encoding.UTF8.GetBytes("<html><body><h3>You may close this window</h3></body></html>");
            response.Headers.Clear();
            response.StatusCode = (int)HttpStatusCode.OK;
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            response.Close();

            await Task.Delay(100);
            listener.Stop();

            return code;
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<string> GetAccessToken(string code, string refresh_token, string grant_type)
        {
            var authRequest = new AuthorizationRequest
            {
                client_id = _configuration.ClientId,
                client_secret = _configuration.ClientSecret,
                code = code,
                refresh_token = refresh_token,
                grant_type = grant_type
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://www.strava.com/oauth/token"),
                Content = new StringContent(JsonSerializer.Serialize(authRequest))
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                }
            };
            using var response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _configuration.RefreshToken = null;
                _configuration.Save();
                throw new UnauthorizedException("Strava refresh token is expired. Please run again to log in.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<AuthorizationResponse>(json);

            _configuration.RefreshToken = auth.refresh_token;
            _configuration.Save();

            return auth.access_token;
        }

        private bool Configured => _configuration.Configured;
    }


#pragma warning disable IDE1006 // Naming Styles
    public class AuthorizationRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string code { get; set; }
        public string refresh_token { get; set; }
        public string grant_type { get; set; } = "authorization_code";
    }

    public class AuthorizationResponse
    {
        public string token_type { get; set; }
        public int expires_at { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }
    }

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
        public double suffer_score { get; set; }

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
                SufferScore = suffer_score
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
