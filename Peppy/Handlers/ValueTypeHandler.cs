using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Peppy.Interfaces;

namespace Peppy.Handlers
{
   public class ValueTypeHandler : ITypeHandler
   {
      #region IValueTypeHandler implementation
      public void SetValue(IDbDataParameter parameter, object value, Type desiredType)
      {
         // finds DbType for the value type.
         bool mapped = Mapper._typeMap.TryGetValue(desiredType, out DbType dbType);
         parameter.DbType = mapped ? dbType : DbType.String;

         // converts the value to target type.
         parameter.Value = Utility.To(desiredType, value);

         // special case for byte[]. it needs size.
         if (value is byte[]) parameter.Size = ((byte[])value).Length;
      }

      public object Parse(object databaseValue, Type desiredType) => Utility.To(desiredType, databaseValue);
      #endregion

   }
}
