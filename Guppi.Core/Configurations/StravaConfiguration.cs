using Guppi.Core.Attributes;

namespace Guppi.Core.Configurations;

public class StravaConfiguration : Configuration
{
    [Display("Client Id")]
    public string ClientId { get; set; }

    [Display("Client Secret")]
    public string ClientSecret { get; set; }

    [Hide]
    public string RefreshToken { get; set; }
}
