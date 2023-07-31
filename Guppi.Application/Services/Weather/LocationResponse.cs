namespace Guppi.Application.Services.Weather;

public class LocationResponse
{
#pragma warning disable IDE1006 // Naming Styles
    public string name { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    public Domain.Entities.Weather.Location GetLocation() => new()
    {
        Name = name,
        State = state,
        Country = country,
        Latitude = lat.ToString(),
        Longitude = lon.ToString()
    };
}
