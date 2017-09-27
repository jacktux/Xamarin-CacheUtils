using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ib.Xamarin.CacheUtils.CacheRestService
{
    public interface ICacheRestService
    {
        Task<T> GetDataAsync<T>(string url);

        Task<T> PostDataAsync<T>(object item, string apiUrl);

    }
}
