using Peppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Peppy.Readers
{
   /// <summary>
   /// This reader reads a records from IDbCommand and 
   /// maps a data to the T template object which can be
   /// Typed-Class or Anonymous class object.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class QueryGenericReader<T>
   {
      public List<T> Read(IDbCommand command, ExecuteArgs args)
      {
         List<T> result = new List<T>();

         using (IDataReader reader = command.ExecuteReader())
         {
            Dictionary<string, int> lookupIndex = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++) lookupIndex[reader.GetName(i).ToLower()] = i;

            // pagination
            int offset = args.Offset ?? 0;
            int limit = args.Limit ?? int.MaxValue;

            while (reader.Read())
            {
               if (offset > 0) { offset--; continue; }

               T item = Activator.CreateInstance<T>();
               foreach (PropertyInfo pi in typeof(T).GetProperties())
               {
                  // verifies whether the property name matches column name.
                  bool success = lookupIndex.TryGetValue(pi.Name.ToLower(), out int index);
                  if (!success) continue;

                  // ignores if the property has [IgnoreFetch]
                  var ignore = pi.GetCustomAttribute<IgnoreFetchAttribute>();
                  if (ignore != null) continue;

                  // reads column value.
                  object value = reader.GetValue(index);
                  if (value == DBNull.Value) continue; // 2019-08-15: issue with Guid

                  // assign the database value to the property.
                  pi.SetValue(item, Utility.To(pi.PropertyType, value));
               }

               result.Add(item);
               if (--limit <= 0) break;

            }
         }

         return result;
      }
   }
}
