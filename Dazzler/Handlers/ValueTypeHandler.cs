using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dazzler.Interfaces;

namespace Dazzler.Handlers
{
   public class ValueTypeHandler : ITypeHandler
   {
      #region IValueTypeHandler implementation
      public void SetValue(IDbDataParameter parameter, object value, Type desiredType)
      {
         // converts the value to target type.
         parameter.Value = Utility.To(desiredType, value, out Type underlyingType, DBNull.Value);


         // finds DbType for the value type.
         bool mapped = Mapper._typeMap.TryGetValue(underlyingType, out DbType dbType);
         parameter.DbType = mapped ? dbType : DbType.String;
         

         // special case for byte[]. it needs size.
         if (value is byte[]) parameter.Size = ((byte[])value).Length;
      }

      public object Parse(object databaseValue, Type desiredType) => Utility.From(desiredType, databaseValue);
      #endregion
   }
}
