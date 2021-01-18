using System;
using System.Data;
using Peppy.Interfaces;


namespace Peppy.Handlers
{
   public class DataTableValueHandler : ITypeHandler
   {
      #region IValueTypeHandler implementation
      public void SetValue(IDbDataParameter parameter, object value, Type desiredType)
      {
         if (!(desiredType == typeof(DataTable))) throw new ArgumentException();

         DataTable dataTable = value as DataTable;

         // for SqlServer
         string sqlPropName = "SqlDbType";
         var pi = Utility.GetProperty(parameter, sqlPropName);
         if (pi != null)
         {
            parameter.Value = value;
            Utility.SetPropertyValue(pi, sqlPropName, (int)SqlDbType.Structured);
            Utility.SetPropertyValue(pi, "TypeName", dataTable?.TableName);
            return;
         }

         // for Oracle
         string oraclePropName = "OracleDbType";
         pi = Utility.GetProperty(parameter, oraclePropName);
         if (pi != null)
         {
            // Oracle RefCursor needed.
            // Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor = 121
            Utility.SetPropertyValue(pi, oraclePropName, 121);
            parameter.Value = value; // !!!! complete this !!!!

            return;
         }
      }

      public object Parse(object value, Type desiredType) => null;
      #endregion
   }
}
