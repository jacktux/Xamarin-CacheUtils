using Akavache;
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
        private ICacheRestService restService;
        public event EventHandler<CacheEventArgs> CacheUpdated;
        public delegate void CacheEventHandler(object sender, CacheEventArgs e);

        public CacheRestManager(ICacheRestService service)
        {
            restService = service;
        }

        public virtual async Task<T> GetRestDataAsync<T>(string url, string eventListenerTag)
        {
            return await _GetRestDataAsync<T>(url, false, eventListenerTag);
        }

        public virtual async Task<T> GetRestDataAsync<T>(string url, bool forceRefresh = false)
        {
            return await _GetRestDataAsync<T>(url, false, null);
        }

        public IObservable<T> GetPosts<T>(string url)
        {
            return CacheUtils.Cache.GetAndFetchLatest(url, () => restService.GetDataAsync<T>(url),
                offset =>
                {
                    TimeSpan elapsed = DateTimeOffset.Now - offset;
                    return elapsed > CacheUtils.CACHE_HOLD_TIME;
                });
        }

        private async Task<T> _GetRestDataAsync<T>(string url, bool forceRefresh = false, string tag = null)
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

            bool newResult = false;
            cachedPosts.Subscribe(subscribedPost =>
            {
                result = subscribedPost;
                if (newResult)
                {
                    CacheEventArgs e = new CacheEventArgs(subscribedPost);
                    e.EventListenerTag = tag ?? "";
                    OnCacheUpdated(e);
                }
                newResult = true;
            });

            result = await cachedPosts.FirstOrDefaultAsync();
            Debug.WriteLine($"{url} - {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public virtual Task<T> PostRestDataAsync<T>(object item, string url)
        {
            return restService.PostDataAsync<T>(item, url);
        }

        protected virtual void OnCacheUpdated(CacheEventArgs e)
        {
            CacheUpdated?.Invoke(this, e);
        }
    }

    public class CacheEventArgs : EventArgs
    {
        public object Results;
        public string EventListenerTag;

        public CacheEventArgs(object results)
        {
            Results = results;
        }
    }
}
