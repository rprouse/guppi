namespace Guppi.Core.Entities.Weather;

/// <summary>
/// A location response from the weather provider for a geocode lookup
/// </summary>
public class Location
{
    public string Name { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(State))
            return $"{Name}, {Country} ({Latitude},{Longitude})";

        return $"{Name}, {State}, {Country} ({Latitude},{Longitude})";
    }
}
