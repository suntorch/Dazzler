using System;
using System.Data;


namespace Dazzler.Models
{
   public enum ExecutionType
   {
      Query, NonQuery, Scalar
   }

   public class CommandArgs : CommandInfo
   {
      public IDbTransaction Transaction { get; set; }

      /// <summary>
      /// Specifies execution method type.
      /// </summary>
      public ExecutionType ExecutionType { get; set; }

      /// <summary>
      /// Specifies whether to invoke an execution events ExecutingEvent and ExecutedEvent.
      /// </summary>
      public bool? NoEvent { get; set; }

      /// <summary>
      /// Specifies whether to store the result in the cache for the future retrieval.
      /// </summary>
      public Cache Cache { get; set; }

   }
}
