using System;
using System.Collections.Generic;
using System.Text;


namespace Peppy.Models
{
   public class ResultInfo
   {
      /// <summary>
      /// Gets an affected row count of the execution.
      /// </summary>
      public int AffectedRows { get; set; }

      /// <summary>
      /// Gets total elapsed time of the execution, in milliseconds.
      /// </summary>
      public long Duration { get; set; }

      /// <summary>
      /// Indicates the result came from cache storage instead of database.
      /// </summary>
      public bool FromCache { get; set; }
   }
}
