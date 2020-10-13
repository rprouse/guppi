﻿using Guppi.Core;
using Guppi.Core.Attributes;

namespace DataProvider.Weather
{
    class WeatherConfiguration : Configuration
    {
        [Display("API Key")]
        public string ApiKey { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}
