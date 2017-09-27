using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;

namespace Ib.Xamarin.CacheUtils.ObjectCache
{
    public abstract class ObjectStorage
    {

        public async Task<Unit> SaveAsync()
        {
            Type type = GetType();
            return await CacheUtils.Cache.InsertObject<object>(type.Name, this);
        }

    }
}
