using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Peppy.Interfaces
{
   public interface IQueryReader
   {
      object Read(IDbCommand command, ExecuteArgs args);
   }
}
