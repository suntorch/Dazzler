using System;
using Dazzler.Interfaces;


namespace Dazzler
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
