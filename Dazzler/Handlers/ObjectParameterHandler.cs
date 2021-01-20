using System;
using System.Reflection;
using Dazzler.Interfaces;

namespace Dazzler.Handlers
{
   /// <summary>
   /// This implementation is used to map a Strongly Typed Class and
   /// Anonymous Class object that is passed to database command as parameter. 
   /// A class member name should match with query parameter name and 
   /// can be include suffixes to specify an attributes of query parameter.
   /// </summary>
   public class ObjectParameterHandler : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(object);

      public void ForEach(object parameterObject, Action<string, object, Type> action)
      {
         if (parameterObject == null || action == null) return;

         Type type = parameterObject.GetType();
         foreach (PropertyInfo pi in type.GetProperties())
         {
            // exception if member implemented IgnoreFetch attribute.
            if (pi.GetCustomAttribute<IgnoreFetchAttribute>(false) != null) continue; // skip

            action?.Invoke(pi.Name, pi.GetValue(parameterObject), pi.PropertyType);
         }
      }

      public void SetValue(object parameterObject, string name, object value) => Utility.SetBackiingFieldValue(parameterObject, name, value);

   }
}
