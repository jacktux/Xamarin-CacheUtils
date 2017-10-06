using Akavache;
using Ib.Xamarin.CacheUtils.CacheRestService.RestServiceAttribute;
using Ib.Xamarin.CacheUtils.RestServiceAttribute.CacheRestService;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">DTO object</typeparam>
        /// <param name="url">REST Endpoint</param>
        /// <param name="eventListenerTag">Tag to Id the Cache update event callback</param>
        /// <returns></returns>
        public virtual async Task<T> GetRestDataAsync<T>(string url, string eventListenerTag) where T : new()
        {
            return await _GetRestDataAsync<T>(url, false, eventListenerTag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">DTO object</typeparam>
        /// <param name="url">REST Endpoint</param>
        /// <param name="forceRefresh">true to get a fresh copy of the data and insert into the cache</param>
        /// <returns></returns>
        public virtual async Task<T> GetRestDataAsync<T>(string url, bool forceRefresh = false) where T : new()
        {
            return await _GetRestDataAsync<T>(url, forceRefresh, null);
        }

        private async Task<T> _GetRestDataAsync<T>(string url, bool forceRefresh = false, string tag = null) where T : new()
        {
            var result = default(T);
            try
            {
                //Offline
                if (!CrossConnectivity.Current.IsConnected)
                    return await CacheUtils.Cache.GetOrCreateObject<T>(url, () => { return new T(); });

                //Online
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
                        return elapsed > typeof(T).GetDtoHoldTimeOrDefault();
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// To POST your object to the url Endpoint
        /// </summary>
        /// <typeparam name="T">DTO object</typeparam>
        /// <param name="item">Object of type T</param>
        /// <param name="url">REST Endpoint</param>
        /// <returns></returns>
        public virtual Task<T> PostRestDataAsync<T>(object item, string url)
        {
            try
            {
                return restService.PostDataAsync<T>(item, url);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected virtual void OnCacheUpdated(CacheEventArgs e)
        {
            CacheUpdated?.Invoke(this, e);
        }
    }
}
