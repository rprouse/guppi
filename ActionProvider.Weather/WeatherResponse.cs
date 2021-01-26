namespace ActionProvider.Weather
{
#pragma warning disable IDE1006 // Naming Styles
    public class WeatherResponse
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public string timezone { get; set; }
        public Current current { get; set; }
        public Hourly[] hourly { get; set; }
        public Daily[] daily { get; set; }
    }

    public class Current
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public float wind_gust { get; set; }
        public Weather[] weather { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Hourly
    {
        public long dt { get; set; }
        public float temp { get; set; }
        public float feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public int clouds { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public Weather[] weather { get; set; }
        public float pop { get; set; }
    }

    public class Daily
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public Temp temp { get; set; }
        public Feels_Like feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public Weather[] weather { get; set; }
        public int clouds { get; set; }
        public float uvi { get; set; }
        public float rain { get; set; }
        public float snow { get; set; }
    }

    public class Temp
    {
        public float day { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float night { get; set; }
        public float eve { get; set; }
        public float morn { get; set; }
    }

    public class Feels_Like
    {
        public float day { get; set; }
        public float night { get; set; }
        public float eve { get; set; }
        public float morn { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
