using System;

namespace Guppi.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTimeOffset GetRfc3339Date(this string date) =>
        DateTimeOffset.TryParse(date, out DateTimeOffset result) ? result : DateTimeOffset.Now;
}
