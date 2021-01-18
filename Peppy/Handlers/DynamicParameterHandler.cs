using System;
using System.Reflection;
using Peppy.Interfaces;

namespace Peppy.Handlers
{
   public class DynamicParameterHandler : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(object);

      public void ForEach(object parameterObject, Action<string, object, Type> action)
      {
         if (parameterObject == null || action == null) return;

         Type type = parameterObject.GetType();
         PropertyInfo[] properties = type.GetProperties();
         for (int i = 0; i < properties.Length; i++)
         {
            PropertyInfo pi = properties[i];

            // exceptions
            if (pi.GetCustomAttribute<IgnoreFetchAttribute>(false) != null) continue; // skip

            action?.Invoke(pi.Name, pi.GetValue(parameterObject), pi.PropertyType);
         }
      }

      public void SetValue(object parameterObject, string name, object value) => Utility.SetBackiingFieldValue(parameterObject, name, value);

   }
}
