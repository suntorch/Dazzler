using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using System.Dynamic;
using Peppy.Interfaces;
using Peppy.Handlers;
using Peppy.Readers;

namespace Peppy
{
   public static partial class Mapper
   {

      #region public - events

      public delegate void ExecutingDelegate(EventArgs args);
      public delegate void ExecutedDelegate(EventArgs args, ResultInfo result);

      public static event ExecutingDelegate ExecutingEvent;
      public static event ExecutedDelegate ExecutedEvent;

      public static void OnExecutingEvent(EventArgs args) => ExecutingEvent?.Invoke(args);
      public static void OnExecutedEvent(EventArgs args, ResultInfo result) => ExecutedEvent?.Invoke(args, result);

      #endregion

      #region public - properties

      public static string ParameterPrefix { get; set; } // if it's null, mapper detects automatically.
      public static int? CommandTimeout { get; set; }
      #endregion

      #region public - methods

      public static void Use<T>(IParameterHandler handler) where T : new()
      {
         _objectHandlers[typeof(T)] = handler;
      }
      public static void Use<T>(ITypeHandler handler) where T : new()
      {
         _valueHandlers[typeof(T)] = handler;
      }



      public static List<T> Query<T>(this IDbConnection conn, ExecuteArgs args, ResultInfo ri)
         => ExecuteImpl<T>(conn, args, ri, new QueryGenericReader<T>());


      public static List<T> Query<T>(this IDbConnection conn,
         CommandType kind,
         string sql,
         object data = null,
         int? offset = null,
         int? limit = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => Query<T>(conn, new ExecuteArgs()
         {
            Kind = kind,
            Sql = sql,
            Data = data,
            State = state,
            Offset = offset,
            Limit = limit,
            Timeout = timeout,
            NoEvent = noevent
         }, ri);


      public static List<T> Query<T>(this IDbConnection conn,
         string sql,
         object data = null,
         int? offset = null,
         int? limit = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => Query<T>(conn, CommandType.StoredProcedure, sql, data, offset, limit, timeout, noevent, state, ri);



      public static int NonQuery(this IDbConnection conn, ExecuteArgs args, ResultInfo ri)
         => ExecuteImpl(conn, args, ri);


      public static int NonQuery(this IDbConnection conn,
         CommandType kind,
         string sql,
         object data = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => NonQuery(conn, new ExecuteArgs()
         {
            Kind = kind,
            Sql = sql,
            Data = data,
            State = state,
            Timeout = timeout,
            NoEvent = noevent
         }, ri);


      public static int NonQuery(this IDbConnection conn,
         string sql,
         object data = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => NonQuery(conn, CommandType.StoredProcedure, sql, data, timeout, noevent, state, ri);


      #endregion

   }
}
