using System;
using System.Reflection;
using Peppy.Interfaces;

namespace Peppy.Handlers
{
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
