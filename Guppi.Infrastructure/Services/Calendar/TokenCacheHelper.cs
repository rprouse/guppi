using System.IO;
using System.Threading.Tasks;
using Guppi.Application;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Guppi.Infrastructure.Services.Calendar
{
    static class TokenCacheHelper
    {
        internal static async Task RegisterCache(ITokenCache tokenCache)
        {
            var cacheFile = Configuration.GetConfigurationFile("o365.msalcache", "bin");
            var cacheFileName = Path.GetFileName(cacheFile);
            var cachePath = Path.GetDirectoryName(cacheFile);
            var storageProperties = new StorageCreationPropertiesBuilder(cacheFileName, cachePath).Build();
            var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            cacheHelper.RegisterCache(tokenCache);
        }
    }
}
