using Dazzler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dazzler.Models
{
   public class Cache
   {
      #region internals
      /// <summary>
      /// only used to pass the cache object instance of the DbContext to the Mapper.
      /// </summary>
      internal object _instance = null;
      internal void Init(DbContextOptions options)
      {
         _instance = options?._cacheInstance;
         this.Expiration ??= options._cacheExpiration;
      }
      #endregion

      #region constructors
      public Cache() { }
      public Cache(int seconds) => this.Expiration = seconds;
      #endregion

      #region properties
      public int? Expiration { get; set; }
      #endregion
   }
}
