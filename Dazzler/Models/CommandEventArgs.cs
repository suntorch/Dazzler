using System;
using System.Collections.Generic;
using System.Text;


namespace Dazzler.Models
{
   public class CommandEventArgs : CommandInfo
   {
      /// <summary>
      /// Specifies to reject the execution.
      /// </summary>
      public bool Cancel { get; set; }
   }
}
