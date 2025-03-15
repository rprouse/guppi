using System;
using Guppi.Core.Entities.Weather;
using Guppi.Core.Extensions;

namespace Guppi.Core.Services;

/// <summary>
/// Sunrise and sunset calculator
/// </summary>
/// <remarks>
/// Based on https://en.wikipedia.org/wiki/Sunrise_equation
/// https://en.wikipedia.org/wiki/Sunrise_equation#Complete_calculation_on_Earth
/// </remarks>
/// <param name="logger"></param>
public static class SunriseService
{
    /// <summary>
    /// Calculates the sunrise and sunset times for a given date, latitude, longitude, and elevation.
    /// </summary>
    /// <param name="day">The date for which to calculate the sunrise and sunset times.</param>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <param name="elevation">The elevation of the location in meters.</param>
    /// <param name="debugTz">The time zone to use for debugging purposes.</param>
    /// <returns>A <see cref="SunriseResult"/> object containing the sunrise and sunset times, and whether it is a polar day.</returns>
    /// <exception cref="ArgumentException">Thrown when the sun is up all day (polar day).</exception>
    public static SunriseResult Calculate(
        DateTimeOffset day,
        double latitude,
        double longitude,
        double elevation,
        TimeZoneInfo debugTz)
    {
        // Get timestamp (Unix epoch time)
        double currentTimestamp = day.ToUnixTimeSeconds();

        double jDate = TimestampToJulian(currentTimestamp);

        // Julian day
        double n = Math.Ceiling(jDate - (2451545.0 + 0.0009) + 69.184 / 86400.0);

        // Mean solar time
        double j_ = n + 0.0009 - longitude / 360.0;

        // Solar mean anomaly
        double mDegrees = (357.5291 + 0.98560028 * j_) % 360;
        double mRadians = mDegrees.Radians();

        // Equation of the center
        double cDegrees = 1.9148 * Math.Sin(mRadians) + 0.02 * Math.Sin(2 * mRadians) + 0.0003 * Math.Sin(3 * mRadians);

        // Ecliptic longitude
        double lDegrees = (mDegrees + cDegrees + 180.0 + 102.9372) % 360;

        double lambdaRadians = lDegrees.Radians();

        // Solar transit (Julian date)
        double jTransit = (2451545.0 + j_ + 0.0053 * Math.Sin(mRadians) - 0.0069 * Math.Sin(2 * lambdaRadians));

        // Declination of the Sun
        double sinD = Math.Sin(lambdaRadians) * Math.Sin(23.4397.Radians());
        double cosD = Math.Cos(Math.Asin(sinD));

        // Hour angle
        double someCos = (Math.Sin((-0.833 - 2.076 * Math.Sqrt(elevation) / 60.0).Radians()) - Math.Sin(latitude.Radians()) * sinD)
                        / (Math.Cos(latitude.Radians()) * cosD);

        try
        {
            double w0Radians = Math.Acos(someCos);
            double w0Degrees = w0Radians.Degrees(); // 0...180


            double jRise = jTransit - w0Degrees / 360;
            double jSet = jTransit + w0Degrees / 360;

            return new SunriseResult
            {
                Sunrise = ConvertUnixToLocalTime(JulianToTimestamp(jRise), debugTz),
                Sunset = ConvertUnixToLocalTime(JulianToTimestamp(jSet), debugTz),
                IsPolarDay = false
            };
        }
        catch (ArgumentException)
        {
            return new SunriseResult
            {
                Sunrise = day.Date,
                Sunset = day.AddDays(1).Date,
                IsPolarDay = true
            };
        }
    }

    private static DateTimeOffset ConvertUnixToLocalTime(double ts, TimeZoneInfo debugTz)
    {
        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds((long)ts);
        if (debugTz != null)
        {
            return TimeZoneInfo.ConvertTime(dateTime, debugTz);
        }
        return dateTime;
    }

    public static double JulianToTimestamp(double j) =>
        (j - 2440587.5) * 86400;

    public static double TimestampToJulian(double ts) =>
        ts / 86400.0 + 2440587.5;
}
