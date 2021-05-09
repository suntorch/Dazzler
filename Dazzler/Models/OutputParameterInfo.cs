using System;
using System.Data;
using System.Collections.Generic;
using Dazzler.Interfaces;


namespace Dazzler
{
   /// <summary>
   /// internal use only.
   /// </summary>
   internal class OutputParameterInfo
   {
      internal IParameterHandler parameterHandler;
      internal string parameterName;

      internal string propertyName;
      internal Type propertyType;
      internal ITypeHandler typeHandler;
      internal object value;
   }
}
