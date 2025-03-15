using System;

namespace Guppi.Core.Entities.Weather;

public struct SunriseResult
{
    public DateTimeOffset Sunrise { get; set; }
    public DateTimeOffset Sunset { get; set; }
    public bool IsPolarDay { get; set; }
    public TimeSpan DayLength => Sunset - Sunrise;

    public override string ToString() =>
        IsPolarDay ? "Sun is up all day" : $"Sunrise: {Sunrise}{Environment.NewLine}Sunset:  {Sunset}{Environment.NewLine}Length:  {DayLength} hours";
}
