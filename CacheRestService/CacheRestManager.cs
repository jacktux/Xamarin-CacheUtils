﻿using Akavache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ib.Xamarin.CacheUtils.CacheRestService
{
    public class CacheRestManager
    {
        ICacheRestService restService;

        public CacheRestManager(ICacheRestService service)
        {
            restService = service;
        }

        public virtual async Task<T> GetRestDataAsync<T>(string url, bool forceRefresh = false)
        {
            var result = default(T);

            if (forceRefresh)
            {
                result = await restService.GetDataAsync<T>(url);
                Debug.WriteLine($"FORCE: {url} - {JsonConvert.SerializeObject(result)}");
                if (result != null)
                    await CacheUtils.Cache.InsertObject<T>(url, result);
                return result;
            }

            var cachedPosts = CacheUtils.Cache.GetAndFetchLatest(url, () => restService.GetDataAsync<T>(url),
                offset =>
                {
                    TimeSpan elapsed = DateTimeOffset.Now - offset;
                    return elapsed > CacheUtils.CACHE_HOLD_TIME;
                });
            cachedPosts.Subscribe(subscribedPosts =>
            {
                result = subscribedPosts;
            });

            result = await cachedPosts.FirstOrDefaultAsync();
            Debug.WriteLine($"{url} - {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public virtual Task<T> PostRestDataAsync<T>(object item, string url)
        {
            return restService.PostDataAsync<T>(item, url);
        }

    }
}