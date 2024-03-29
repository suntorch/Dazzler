﻿using System;
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

      public void ForEach(object parameterObject, ParameterOptions options)
      {
         if (parameterObject == null || options == null) return;

         Type type = parameterObject.GetType();
         foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
         {
            // exception if member implemented IgnoreFetch attribute.
            if (pi.GetCustomAttribute<IgnoreFetchAttribute>(false) != null) continue; // skip

            var attr = pi.GetCustomAttribute<BindAttribute>(false);

            options?.Invoke(pi.Name, pi.GetValue(parameterObject), pi.PropertyType, attr);
         }
      }

      public void SetValue(object parameterObject, string name, object value)
      {
         if (parameterObject is null) return;
         if (parameterObject.GetType().Name.Contains("AnonymousType"))
            Utility.SetBackiingFieldValue(parameterObject, name, value);
         else
            Utility.SetPropertyValue(parameterObject, name, value);
      }
   }
}
