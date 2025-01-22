using System.Threading.Tasks;
using Guppi.Core.Entities.Rss;

namespace Guppi.Core.Interfaces.Services;

public interface IRssService
{
    void Configure();

    Task<NewsFeed> ReadRssFeed(string inputUri);
}
