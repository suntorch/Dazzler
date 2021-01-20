using System;
using System.Data;
using System.Collections.Generic;
using Peppy.Interfaces;


namespace Peppy
{
   /// <summary>
   /// internal use only.
   /// </summary>
   internal class ParameterLookup
   {
      internal Dictionary<IDbDataParameter, ParameterLookupData> map;
      internal IParameterHandler parameterHandler;
   }
}
