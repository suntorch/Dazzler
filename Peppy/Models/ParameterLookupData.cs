using System;
using Peppy.Interfaces;


namespace Peppy
{
   /// <summary>
   /// internal use only.
   /// </summary>
   internal class ParameterLookupData
   {
      internal string name;
      internal Type type;
      internal ITypeHandler typeHandler;
   }
}
