using System;
using System.Data;


namespace Peppy.Models
{
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
      /// Specifies an user state to pass any data to an events and results.
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
