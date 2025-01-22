using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Edi.SyndicationFeed.ReaderWriter;
using Edi.SyndicationFeed.ReaderWriter.Rss;
using Guppi.Core.Configurations;
using Guppi.Core.Entities.Rss;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Core.Services;

public class RssService : IRssService
{
    public void Configure()
    {
        var configuration = Configuration.Load<RssConfiguration>("Rss");
        configuration.RunConfiguration("Rss", "Configure your RSS feeds.");
    }

    public async Task<NewsFeed> ReadRssFeed(string inputUri)
    {
        NewsFeed feed = new ();
        using var xmlReader = XmlReader.Create(inputUri, new XmlReaderSettings() { Async = true });
        var feedReader = new RssFeedReader(xmlReader);
        var parser = new RssParser();

        while (await feedReader.Read())
        {
            switch (feedReader.ElementType)
            {
                // Read Item
                case SyndicationElementType.Item:
                    {
                        // Read the item as generic content
                        ISyndicationContent itemContent = await feedReader.ReadContent();
                        ISyndicationItem item = parser.CreateItem(itemContent);
                        ISyndicationContent content = itemContent.Fields.FirstOrDefault(f => f.Namespace == "http://purl.org/rss/1.0/modules/content/" && f.Name == "encoded");
                        ISyndicationContent creator = itemContent.Fields.FirstOrDefault(f => f.Namespace == "http://purl.org/dc/elements/1.1/" && f.Name == "creator");
                        feed.Items.Add(new NewsItem
                        {
                            Id = item.Id ?? string.Empty,
                            Title = item.Title ?? string.Empty,
                            Description = item.Description ?? string.Empty,
                            Published = item.Published,
                            Link = item.Links.FirstOrDefault()?.Uri?.ToString() ?? string.Empty,
                            Content = content?.Value ?? string.Empty,
                            Author = creator?.Value ?? string.Empty
                        });
                    }
                    break;

                // Read content
                default:
                    {
                        ISyndicationContent content = await feedReader.ReadContent();
                        if (content.Namespace != "")
                        {
                            continue;
                        }
                        switch (content.Name.ToLowerInvariant())
                        {
                            case "title":
                                feed.Title = content.Value;
                                break;
                            case "description":
                                feed.Description = content.Value;
                                break;
                            case "link":
                                feed.Link = content.Value;
                                break;
                        }
                    }
                    break;
            }
        }
        return feed;
    }
}
