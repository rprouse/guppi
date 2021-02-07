using Guppi.Application;
using Guppi.Application.Attributes;

namespace ActionProvider.Weather
{
    class WeatherConfiguration : Configuration
    {
        [Display("API Key")]
        public string ApiKey { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
