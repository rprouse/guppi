namespace Guppi.Infrastructure.Services.Strava
{
#pragma warning disable IDE1006 // Naming Styles

    public class AuthorizationResponse
    {
        public string token_type { get; set; }
        public int expires_at { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}
