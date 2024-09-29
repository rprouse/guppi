using System;

namespace Guppi.Application.Extensions;

public static class DataTimeExtensions
{
    public static DateTime GetRfc3339Date(this string date) =>
        DateTime.TryParse(date, out DateTime result) ? result : DateTime.Now;
}
