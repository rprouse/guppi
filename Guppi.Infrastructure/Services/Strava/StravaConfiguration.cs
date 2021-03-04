using Guppi.Application;
using Guppi.Application.Attributes;

namespace Guppi.Infrastructure.Services.Strava
{
    public class StravaConfiguration : Configuration
    {
        [Display("Client Id")]
        public string ClientId { get; set; }

        [Display("Client Secret")]
        public string ClientSecret { get; set; }

        [Hide]
        public string RefreshToken { get; set; }
    }
}
