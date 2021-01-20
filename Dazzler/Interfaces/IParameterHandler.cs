using System;


namespace Dazzler.Interfaces
{
   /// <summary>
   /// Implement this interface to handle user parameter object.
   /// </summary>
   public interface IParameterHandler
   {
      /// <summary>
      /// Implement this interface to specify a type of the ParameterObject.
      /// </summary>
      Type DesiredType { get; }

      /// <summary>
      /// Implement this interface to extract a member values of the parameter object
      /// and invoke an action method for each member in order to assign IDbParameter values.
      /// </summary>
      /// <param name="parameterObject">Parameter object such as Persistant or Anonymous class object or Dictionary etc.</param>
      /// <param name="action">The action method to work for each parameter value to set IDbParameter.</param>
      void ForEach(object parameterObject, Action<string, object, Type> action);

     
      /// <summary>
      /// 
      /// </summary>
      /// <param name="parameterObject"></param>
      /// <param name="name"></param>
      /// <param name="value"></param>
      void SetValue(object parameterObject, string name, object value);
   }

}
