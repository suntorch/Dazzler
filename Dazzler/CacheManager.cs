using Dazzler.Models;
using System;
using System.Text;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
#endif

namespace Dazzler
{
   internal class CacheManager
   {
      private Cache _cache = null;
      private object _instance = null;
      private string _key = null;

      internal CacheManager(CommandArgs args)
      {
         if (args.Cache == null) return;

         _cache = args.Cache;
         _instance = _cache._instance ?? Mapper._cacheInstance;

         // generate key if it's cacheable.
         if (_instance != null)
         {
            _key = Utility.FastSerialize(args.Sql, args.Data);
         }
      }


      internal bool IsCacheable => _instance != null;

      internal bool Get(out CacheItem item)
      {
         item = null;
         if (_instance == null) return false;

         bool success = false;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)

         if (_instance is IMemoryCache mc)
         {
            success = mc.TryGetValue<CacheItem>(_key, out item);
         }
         else if (_instance is IDistributedCache dc)
         {
            string json = dc.GetString(_key);
            item = JsonSerializer.Deserialize<CacheItem>(json);
         }

#endif
         return success;
      }
      internal bool Put(CacheItem item)
      {
         if (_instance == null || item == null) return false;
         bool success = false;
         int expiration = _cache?.Expiration ?? Mapper._cacheExpiration;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)

         if (_instance is IMemoryCache mc)
         {
            var memoryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(TimeSpan.FromSeconds(expiration));

            mc.Set(_key, item, memoryOptions);
         }
         else if (_instance is IDistributedCache dc)
         {
            var distributedOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(expiration));

            dc.SetString(_key, JsonSerializer.Serialize(item), distributedOptions);
         }

#endif

         return success;
      }

   }
}
