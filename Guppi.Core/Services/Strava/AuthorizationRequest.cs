namespace Guppi.Application.Services.Strava
{
#pragma warning disable IDE1006 // Naming Styles
    public class AuthorizationRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string code { get; set; }
        public string refresh_token { get; set; }
        public string grant_type { get; set; } = "authorization_code";
    }

#pragma warning restore IDE1006 // Naming Styles
}
