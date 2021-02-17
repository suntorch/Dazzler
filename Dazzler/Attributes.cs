using System;
using System.Collections.Generic;
using System.Text;

namespace Dazzler
{
   /// <summary>
   /// Implement this attribute to the property that does not need to be mapped
   /// to the database returning column.
   /// </summary>
   public class IgnoreFetchAttribute : Attribute { }



   /// <summary>
   /// 
   /// </summary>
   public enum Direction : int { In, Out, InOut, Return }

   /// <summary>
   /// 
   /// </summary>
   public class DirectionAttribute : Attribute
   {
      public Direction Direction { get; set; }
      public DirectionAttribute(Direction direction)
      {
         this.Direction = direction;
      }
   }
}
