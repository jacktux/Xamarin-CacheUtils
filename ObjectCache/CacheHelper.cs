using Akavache;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;


namespace Ib.Xamarin.CacheUtils.ObjectCache
{
    public static class CacheHelper
    {
        /// <summary>
        /// Gets object from Cache of type T or
        /// where if name supplied then by key = name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetCacheObject<T>(string name = null)
        {
            var key = name ?? typeof(T).Name;
            JToken result = (JToken)Task.Run<object>(() => GetObjectAsync(key).Result).Result;
            if (result != null)
                return result.ToObject<T>();

            return default(T);
        }

        public static bool RemoveCacheObject<T>(string name = null)
        {
            var key = name ?? typeof(T).Name;
            return (bool)Task.Run<object>(() => RemoveObjectAsync(key).Result).Result;
        }

        private static async Task<object> GetObjectAsync(string key)
        {
            try
            {
                return await CacheUtils.Cache.GetObject<object>(key);
            }
            catch (Exception)
            {
            }
            return null;
        }

        private static async Task<bool> RemoveObjectAsync(string key)
        {
            try
            {
                await CacheUtils.Cache.InvalidateObject<object>(key);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

    }
}
