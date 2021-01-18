using System;
using System.Collections.Generic;
using System.Data;
using Peppy.Interfaces;

namespace Peppy
{
   /// <summary>
   /// internal use only.
   /// </summary>
   internal class ParameterLookup
   {
      internal Dictionary<IDbDataParameter, PropertyData> map;
      internal IParameterHandler parameterHandler;
   }

   internal class PropertyData
   {
      internal string name;
      internal Type type;
      internal ITypeHandler typeHandler;
   }
}
