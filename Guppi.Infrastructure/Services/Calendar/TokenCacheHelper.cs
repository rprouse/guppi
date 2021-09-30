using System;
using System.IO;
using System.Security.Cryptography;
using Guppi.Application;
using Microsoft.Identity.Client;

namespace Guppi.Infrastructure.Services.Calendar
{
    static class TokenCacheHelper
    {
        static TokenCacheHelper()
        {
            CacheFilePath = Configuration.GetConfigurationFile("o365.msalcache", "bin");
        }

        /// <summary>
        /// Path to the token cache
        /// </summary>
        public static string CacheFilePath { get; private set; }

        private static readonly object FileLock = new object();

        public static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                if (File.Exists(CacheFilePath))
                {
                    byte[] bytes = File.ReadAllBytes(CacheFilePath);
                    if (OperatingSystem.IsWindows())
                    {
                        bytes = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                    }
                    args.TokenCache.DeserializeMsalV3(bytes);
                }
            }
        }

        public static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    byte[] bytes = args.TokenCache.SerializeMsalV3();
                    if (OperatingSystem.IsWindows())
                    {
                        bytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                    }
                    // reflect changesgs in the persistent store
                    File.WriteAllBytes(CacheFilePath, bytes);
                }
            }
        }

        internal static void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }
    }
}
