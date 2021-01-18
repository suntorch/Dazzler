using Peppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Peppy
{

   public class EventArgs : CommandInfo
   {
   }


   public class ExecuteArgs : CommandInfo
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



   public class CommandInfo
   {
      /// <summary>
      /// Specifies how a SQL command string is interpreted.
      /// </summary>
      public CommandType Kind { get; set; }

      /// <summary>
      /// Specifies a SQL command string to be executed in the database.
      /// </summary>
      public string Sql { get; set; }

      /// <summary>
      /// Specifies an input parameters to be passed to a SQL command.
      /// </summary>
      public dynamic Data { get; set; }

      /// <summary>
      /// Specifies an user state to passed to an events and results.
      /// </summary>
      public object State { get; set; }

      /// <summary>
      /// Specifies command timeout in seconds.
      /// </summary>
      public int? Timeout { get; set; }

      /// <summary>
      /// Specifes a zero based position to start reading records.
      /// </summary>
      public int? Offset { get; set; }

      /// <summary>
      /// Specifies total record count to read from database.
      /// </summary>
      public int? Limit { get; set; }
   }

}
