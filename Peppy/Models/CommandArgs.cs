using System;
using System.Data;


namespace Peppy.Models
{
   public class CommandArgs : CommandInfo
   {
      public IDbTransaction Transaction { get; set; }


      /// <summary>
      /// Enables or disables an events Executing and Executed.
      /// </summary>
      public bool? NoEvent { get; set; }

      /// <summary>
      /// Specifies the result will be stored in the cache for the future retrieval.
      /// </summary>
      public bool? Cacheable { get; set; }

      /// <summary>
      /// Specifies cache lifetime in seconds if the result is cachable.
      /// Value 0 indicates forever.
      /// </summary>
      public int? CacheLifetime { get; set; }

   }
}
