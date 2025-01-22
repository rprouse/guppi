using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guppi.Core.Entities.Rss;

public class NewsFeed
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Link { get; set; } = string.Empty;

    public List<NewsItem> Items { get; } = new List<NewsItem>();
}
