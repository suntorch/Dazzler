using Dazzler.Models;
using System.Data;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Dazzler
{
   public static partial class Mapper
   {
      /// <summary>
      /// It holds static cache implementation in case of not using Dependency Injection.
      /// Should be IMemoryCache or IDistributedCache.
      /// </summary>
      internal static object _cacheInstance = null;
      internal static int _cacheExpiration = 3600; // 1h in seconds.


#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)

      /// <summary>
      /// Specifies whether to use memory caching.
      /// </summary>
      /// <param name="cache"></param>
      public static void UseMemoryCache(this IDbConnection conn, IMemoryCache cache) => _cacheInstance = cache;

      /// <summary>
      /// Specifies whether to use memory caching.
      /// </summary>
      /// <param name="cache"></param>
      /// <param name="expiration">expiration time in seconds.</param>
      public static void UseMemoryCache(this IDbConnection conn, IMemoryCache cache, int expiration) { _cacheInstance = cache; _cacheExpiration = expiration; }

      /// <summary>
      /// Specifies whether to use distributed caching such as Redis etc.
      /// </summary>
      /// <param name="cache"></param>
      public static void UseDistributedCache(this IDbConnection conn, IDistributedCache cache) => _cacheInstance = cache;

      /// <summary>
      /// Specifies whether to use distributed caching such as Redis etc.
      /// </summary>
      /// <param name="cache"></param>
      /// <param name="expiration">expiration time in seconds.</param>
      public static void UseDistributedCache(this IDbConnection conn, IDistributedCache cache, int expiration) { _cacheInstance = cache; _cacheExpiration = expiration; }

#endif
   }
}


