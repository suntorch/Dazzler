using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Dazzler.Interfaces;

namespace Dazzler.Handlers
{
   /// <summary>
   /// This implementation is used to map a MethodInfo object 
   /// that is passed to database command as parameter. 
   /// A object key name should match with query parameter name and 
   /// can be include suffixes to specify an attributes of query parameter.
   /// </summary>
   public class MethodInfoParameterHandler : IParameterHandler
   {
      public Type DesiredType { get; } = typeof(MethodInfo);

      public void ForEach(object parameterObject, ParameterOptions options)
      {
         if (parameterObject == null || options == null) return;
         if (parameterObject.GetType() != DesiredType) throw new ArgumentException();

         var parameters = (parameterObject as MethodInfo).GetParameters();
         foreach (var pi in parameters)
         {
            object value = pi.DefaultValue;

            // need research how to read runtime values of the method parameter.
            // it might be nearly impossible at this time.
            throw new NotImplementedException();

            //BindAttribute attr = pi.IsOut ? new BindAttribute(ParameterDirection.Output) : null;
            //action?.Invoke(pi.Name, value, pi.ParameterType, attr);
         }
      }


      public void SetValue(object parameterObject, string name, object value)
      {
         throw new NotImplementedException();
      }
   }
}
