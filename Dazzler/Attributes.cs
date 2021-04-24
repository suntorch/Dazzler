using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dazzler
{
   /// <summary>
   /// Implement this attribute to the property that does not need to be mapped
   /// to the database returning column.
   /// </summary>
   public class IgnoreFetchAttribute : Attribute { }




   /// <summary>
   /// Specifies a binding information.
   /// </summary>
   public class BindAttribute : Attribute
   {
      public ParameterDirection Direction { get; set; }
      public int Size { get; set; }

      public BindAttribute(ParameterDirection direction)
      {
         this.Direction = direction;
      }
      public BindAttribute(ParameterDirection direction, int size)
      {
         this.Direction = direction;
         this.Size = size;
      }
      public BindAttribute(int size)
      {
         this.Size = size;
      }

   }

}
