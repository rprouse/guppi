using System.Threading.Tasks;
using System.Xml;
using Edi.SyndicationFeed.ReaderWriter;
using Edi.SyndicationFeed.ReaderWriter.Rss;
using Guppi.Core.Entities.Rss;

namespace Guppi.Core.Services;

public class RssService
{
    public async Task<NewsFeed> ReadRssFeed(string inputUri)
    {
        NewsFeed newsFeed = new ();
        using var xmlReader = XmlReader.Create(inputUri, new XmlReaderSettings() { Async = true });

        var feedReader = new RssFeedReader(xmlReader);

        while (await feedReader.Read())
        {
            switch (feedReader.ElementType)
            {
                // Read category
                case SyndicationElementType.Category:
                    ISyndicationCategory category = await feedReader.ReadCategory();
                    break;

                // Read Image
                case SyndicationElementType.Image:
                    ISyndicationImage image = await feedReader.ReadImage();
                    break;

                // Read Item
                case SyndicationElementType.Item:
                    ISyndicationItem item = await feedReader.ReadItem();
                    break;

                // Read link
                case SyndicationElementType.Link:
                    ISyndicationLink link = await feedReader.ReadLink();
                    break;

                // Read Person
                case SyndicationElementType.Person:
                    ISyndicationPerson person = await feedReader.ReadPerson();
                    break;

                // Read content
                default:
                    ISyndicationContent content = await feedReader.ReadContent();
                    break;
            }
        }
        return newsFeed;
    }
}
