using System;
using System.Linq;

namespace Ib.Xamarin.CacheUtils.CacheRestService
{
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
