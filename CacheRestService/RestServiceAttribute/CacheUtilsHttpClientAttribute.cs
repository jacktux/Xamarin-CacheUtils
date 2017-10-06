using System;
using System.Collections.Generic;
using System.Text;

namespace Ib.Xamarin.CacheUtils.RestServiceAttribute.CacheRestService
{
    public class CacheUtilsHttpClientAttribute : Attribute
    {
        /// <summary>
        /// Size in bytes
        /// </summary>
        public long MaxBufferSize;

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        public long Timeout;
    }
}
