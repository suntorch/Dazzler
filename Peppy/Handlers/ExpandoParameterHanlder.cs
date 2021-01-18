using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using Peppy.Interfaces;

namespace Peppy.Handlers
{
   public class ExpandoParameterHanlder : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(ExpandoObject);


      public void ForEach(object parameterObject, Action<string, object, Type> action)
      {
         if (parameterObject == null || action == null) return;
         if (parameterObject.GetType() != DesiredType) throw new ArgumentException();

         IDictionary<string, object> map = (IDictionary<string, object>)parameterObject;
         foreach (string key in map.Keys)
         {
            object value = map[key];
            action?.Invoke(key, value, value?.GetType() ?? typeof(string));
         }
      }

      public void SetValue(object parameterObject, string name, object value)
      {
         IDictionary<string, object> map = (IDictionary<string, object>)parameterObject;
         map[name] = value;
      }
   }
}
