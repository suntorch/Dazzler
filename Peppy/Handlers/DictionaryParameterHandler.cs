using System;
using System.Collections.Generic;
using System.Text;
using Peppy.Interfaces;

namespace Peppy.Handlers
{
   public class DictionaryParameterHandler : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(Dictionary<string, object>);

      public void ForEach(object parameterObject, Action<string, object, Type> action)
      {
         if (parameterObject == null || action == null) return;
         if (parameterObject.GetType() != DesiredType ) throw new ArgumentException();

         Dictionary<string, object> map = parameterObject as Dictionary<string, object>;
         foreach (string key in map.Keys)
         {
            object value = map[key];
            action?.Invoke(key, value, value?.GetType() ?? typeof(string));
         }
      }


      public void SetValue(object parameterObject, string name, object value)
      {
         Dictionary<string, object> map = parameterObject as Dictionary<string, object>;
         map[name] = value;
      }
   }
}
