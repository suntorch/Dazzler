using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Dazzler
{
   public partial class DbContextOptions
   {
#pragma warning disable 0649
      internal object _cacheInstance;
      internal int _cacheExpiration;
#pragma warning restore 0649


      #region public properties
      public string ConnectionString { get; set; }
      #endregion

      #region constructors
      public DbContextOptions() { }
      #endregion

      #region methods

      /// <summary>
      /// Specifies database connection string.
      /// </summary>
      /// <param name="connectionString"></param>
      /// <returns></returns>
      public DbContextOptions Connection(string connectionString)
      {
         this.ConnectionString = connectionString;
         return this;
      }

      #endregion


      #region !!! framework specific code for caching !!!

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)
      private IServiceCollection _services;
      public DbContextOptions(IServiceCollection services) => _services = services;


      /// <summary>
      /// Specifies to use IMemoryCache service.
      /// </summary>
      /// <param name="defaultExpiration">Default expiration time in seconds.</param>
      /// <returns></returns>
      public DbContextOptions UseMemoryCache(int defaultExpiration = 3600)
      {
         _cacheExpiration = defaultExpiration;
         _cacheInstance = _services?.BuildServiceProvider()?.GetServices<IMemoryCache>();
         return this;
      }

      /// <summary>
      /// Specifies to use IDistributedCache service.
      /// </summary>
      /// <param name="defaultExpiration">Default expiration time in seconds.</param>
      /// <returns></returns>
      public DbContextOptions UseDistributedCache(int defaultExpiration = 3600)
      {
         _cacheExpiration = defaultExpiration;
         _cacheInstance = _services?.BuildServiceProvider()?.GetServices<IDistributedCache>();
         return this;
      }
#endif

      #endregion
   }
}
