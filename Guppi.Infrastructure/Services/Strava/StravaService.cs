using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Strava
{
    public class StravaService : HttpService, IStravaService
    {
        const string Command = "strava";
        const string Name = "strava";

        StravaConfiguration _configuration;

        public StravaService()
        {
            _configuration = Configuration.Load<StravaConfiguration>(Command);
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

            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {access_token}");

            TimeSpan t = DateTime.UtcNow.AddDays(-90) - new DateTime(1970, 1, 1);
            int epoch = (int)t.TotalSeconds;
            var activities = await GetData<List<Activity>>($"https://www.strava.com/api/v3/athlete/activities?after={epoch}&per_page=100");
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

            string code = context.Request.QueryString["code"];
            if (string.IsNullOrWhiteSpace(code))
            {
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
            using var response = await Client.SendAsync(request);

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
}
