using System;
using System.Collections.Generic;
using System.Dynamic;
using Dazzler.Interfaces;

namespace Dazzler.Handlers
{
   /// <summary>
   /// This implementation is used to map a ExpandoObject 
   /// that is passed to database command as parameter. 
   /// A object member/key name should match with query parameter name and 
   /// can be include suffixes to specify an attributes of query parameter.
   /// </summary>
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
