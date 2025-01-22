using System;

namespace Guppi.Core.Entities.Rss;

public class NewsItem
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public DateTimeOffset Published { get; set; } = DateTime.MinValue;
}
