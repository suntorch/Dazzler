using System;
using System.Collections.Generic;
using System.Text;
using Dazzler.Interfaces;

namespace Dazzler.Handlers
{
   /// <summary>
   /// This implementation is used to map a Dictionary object 
   /// that is passed to database command as parameter. 
   /// A object key name should match with query parameter name and 
   /// can be include suffixes to specify an attributes of query parameter.
   /// </summary>
   public class DictionaryParameterHandler : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(Dictionary<string, object>);

      public void ForEach(object parameterObject, Action<string, object, Type, BindAttribute> action)
      {
         if (parameterObject == null || action == null) return;
         if (parameterObject.GetType() != DesiredType ) throw new ArgumentException();

         Dictionary<string, object> map = parameterObject as Dictionary<string, object>;
         foreach (string key in map.Keys)
         {
            object value = map[key];
            action?.Invoke(key, value, value?.GetType() ?? typeof(string), null);
         }
      }


      public void SetValue(object parameterObject, string name, object value)
      {
         Dictionary<string, object> map = parameterObject as Dictionary<string, object>;
         map[name] = value;
      }
   }
}
