using Ib.Xamarin.CacheUtils.CacheRestService.RestServiceAttribute;
//using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ib.Xamarin.CacheUtils.CacheRestService
{
    public class CacheRestService : ICacheRestService, IDisposable
    {
        HttpClient client;

        public CacheRestService(string apiUsername = null, string apiPassword = null)
        {
            client = new HttpClient(); //new NativeMessageHandler()

            if (apiUsername != null)
            {
                var authData = string.Format("{0}:{1}", apiUsername, apiPassword);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            }

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            if (client != null)
                client.Dispose();
        }

        public async Task<T> GetDataAsync<T>(string apiUrl)
        {
            var items = default(T);
            var uri = new Uri(apiUrl);

            try
            {
                client.MaxResponseContentBufferSize = typeof(T).GetDtoMaxBufferSizeOrDefault();
                client.Timeout = typeof(T).GetDtoTimeoutOrDefault();
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    items = JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }

            return items;
        }

        public async Task<T> PostDataAsync<T>(object item, string apiUrl)
        {
            var uri = new Uri(apiUrl);

            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(results);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }

            return default(T);
        }

    }
}
