using System.Threading.Tasks;
using Guppi.Core.Services;
using NUnit.Framework;

namespace Guppi.Tests.Services;

public class RssServiceTests
{
    [Test]
    public async Task ReadRssFeed_WhenCalled_ReturnsRssFeed()
    {
        // Arrange
        var rssService = new RssService();
        var filePath = "Rss/bbc_world_rss.xml";
        //filePath = "https://feeds.bbci.co.uk/news/world/rss.xml";
        // Act
        await rssService.ReadRssFeed(filePath);
        // Assert
        Assert.Pass();
    }
}
