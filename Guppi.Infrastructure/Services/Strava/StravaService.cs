using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Guppi.Application;
using Guppi.Application.Exceptions;
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
            if (string.IsNullOrWhiteSpace(_configuration.RefreshToken) && await Authorize() == false)
            {
                throw new UnauthorizedException("Failed to log into Strava");
            }

            //string json = await _client.GetStringAsync($"http://api.openweathermap.org/data/2.5/onecall?lat={_configuration.Latitude}&lon={_configuration.Longitude}&appid={_configuration.ApiKey}");
            //var weather = JsonSerializer.Deserialize<WeatherResponse>(json);
            //return weather.GetWeather();

            return Enumerable.Empty<Domain.Entities.Strava.StravaActivity>();
        }

        private async Task<bool> Authorize()
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
            string refresh_token = context.Request.QueryString["code"];
            if(string.IsNullOrWhiteSpace(refresh_token))
            {
                result = false;
                byte[] bytes = Encoding.UTF8.GetBytes("<html><body><h2>Failed to log in to Strava</h2><h3>You may close this window</h3></body></html>");
                response.Headers.Clear();
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                response.Close();
            }
            else
            {
                _configuration.RefreshToken = refresh_token;
                _configuration.Save();

                result = true;
                byte[] bytes = Encoding.UTF8.GetBytes("<html><body><h3>You may close this window</h3></body></html>");
                response.Headers.Clear();
                response.StatusCode = (int)HttpStatusCode.OK;
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                response.Close();
            }    

            await Task.Delay(100);
            listener.Stop();

            return result;
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

        private bool Configured => _configuration.Configured;
    }
}
