using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Services.Strava;
using Guppi.Core.Entities.Strava;
using Guppi.Core.Interfaces.Providers;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

internal sealed class StravaService(IHttpRestProvider restService, IProcessProvider processService) : IStravaService
{
    private readonly IHttpRestProvider _restService = restService;
    private readonly IProcessProvider _processService = processService;
    private readonly StravaConfiguration _configuration = Configuration.Load<StravaConfiguration>("strava");

    public void Configure()
    {
        var configuration = Configuration.Load<StravaConfiguration>("strava");
        configuration.RunConfiguration("Strava", "Enter the Strava Client Id and Secret");
    }

    public async Task<IEnumerable<Activity>> GetActivities(int days)
    {
        await SetAccessToken();

        TimeSpan t = DateTime.UtcNow.AddDays(-1 - days) - DateTime.UnixEpoch;
        int epoch = (int)t.TotalSeconds;
        var activities = await _restService.GetData<List<StravaActivity>>($"https://www.strava.com/api/v3/athlete/activities?after={epoch}&per_page=200");
        return activities.Select(a => a.GetActivity());
    }

    private async Task SetAccessToken()
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

        _restService.AddHeader("Authorization", $"Bearer {access_token}");
    }

    private async Task<string> Authorize()
    {
        int port = 39428;
        string url = $"https://www.strava.com/oauth/authorize?client_id={_configuration.ClientId}&response_type=code&redirect_uri=http://localhost:{port}&approval_prompt=auto&scope=read,activity:read_all";
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
            _processService.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                _processService.Start("cmd", $"/c start {url}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _processService.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _processService.Start("open", url);
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
        using var response = await _restService.SendAsync(request);

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
