using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Dazzler.Interfaces
{
   /// <summary>
   /// Implement this interface to handle custom type-based parameter and value parsing.
   /// </summary>
   public interface ITypeHandler
   {
      /// <summary>
      /// Specifies how to assign the typed value to database parameter.
      /// </summary>
      /// <param name="parameter">The parameter to configure</param>
      /// <param name="value">Parameter value</param>
      void SetValue(IDbDataParameter parameter, object value, Type desiredType);

      /// <summary>
      /// Specifies how to parse a database value back to a typed value.
      /// </summary>
      /// <param name="databaseValue">The value from the database</param>
      /// <param name="desiredType">The type to parse to</param>
      /// <returns>The typed value</returns>
      object Parse(object databaseValue, Type desiredType);
   }

}
