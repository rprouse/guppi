using Guppi.Application.Attributes;

namespace Guppi.Application.Configurations
{
    public class WeatherConfiguration : Configuration
    {
        [Display("API Key")]
        public string ApiKey { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
