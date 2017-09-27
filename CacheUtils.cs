using Akavache;
using Akavache.Sqlite3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ib.Xamarin.CacheUtils
{
    public static class CacheUtils
    {
        private static IBlobCache _cache;

        public static TimeSpan CACHE_HOLD_TIME = new TimeSpan(hours: 0, minutes: 10, seconds: 0);
        public static long MAX_RESPONSE_CONTENT_BUFFER_SIZE = 1024000;

        public static void Init(string systemPath)
        {
            BlobCache.ApplicationName = "CacheUtils";
            _cache = new SQLitePersistentBlobCache(systemPath + "/CacheUtils.db");
        }

        public static IBlobCache Cache
        {
            get
            {
                if (_cache == null)
                    throw new TypeInitializationException("CacheNotInitialised", new Exception("Please Initialise CacheUtils before using, ex CacheUtils.Init(<systemPath>)"));
                return _cache;
            }
        }
    }
}
