using System;
using System.Collections.Generic;
using System.Linq;

namespace Dazzler
{
   public class Result
   {
      #region public properties

      public bool Success => (this.Code == 0);

      /// <summary>
      /// Gets an error code of the current operation.
      /// </summary>
      public int Code { get; set; }

      /// <summary>
      /// Gets a message that describes the current operation.
      /// </summary>
      public string Message { get; set; }

      /// <summary>
      /// Gets a total record count that is affected by the current operation.
      /// </summary>
      public int AffectedRows { get; set; }

      /// <summary>
      /// Gets a total elapsed time of the current operation in milliseconds.
      /// </summary>
      public long Duration { get; set; }

      #endregion

      #region constructors

      public Result() { }
      public Result(int code) : this(code, null) { }
      public Result(int code, string message) : this(code, message, 0, 0) { }
      public Result(int code, string message, int affected) : this(code, message, affected, 0) { }
      public Result(int code, string message, int affected, int duration) { this.Set(code, message, affected, duration); }

      #endregion

      #region methods

      public void Set(int code, string message, int affected, int duration)
      {
         this.Code = code;
         this.Message = message;
         this.AffectedRows = affected;
         this.Duration = duration;
      }
      public void Set(int code, string message, int affected) => this.Set(code, message, affected, 0);
      public void Set(int code, string message) => this.Set(code, message, 0, 0);
      public void Set(int code) => this.Set(code, null, 0, 0);

      #endregion
   }

   public class Result<T> : Result
   {
      #region public properties

      public T First => (this.Rows == null ? default(T) : Rows.FirstOrDefault());
      public List<T> Rows { get; } = new List<T>();

      #endregion

      #region constructors

      public Result() { }
      public Result(int code) : base(code, null) { }
      public Result(int code, string message) : base(code, message, 0, 0) { }
      public Result(int code, string message, int affected) : base(code, message, affected, 0) { }
      public Result(int code, string message, int affected, int duration) : base(code, message, affected, duration) { }

      #endregion
   }

}
