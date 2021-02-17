using System;
using System.Data;
using System.Collections.Generic;
using Dazzler.Interfaces;
using Dazzler.Models;
using Dazzler.Readers;

namespace Dazzler
{
   public static partial class Mapper
   {

      #region events

      public delegate void ExecutingDelegate(CommandEventArgs args);
      public delegate void ExecutedDelegate(CommandEventArgs args, ResultInfo result);

      public static event ExecutingDelegate ExecutingEvent;
      public static event ExecutedDelegate ExecutedEvent;

      public static void OnExecutingEvent(CommandEventArgs args)
      {
         ExecutingEvent?.Invoke(args);
         if (args.Cancel) throw new OperationCanceledException();
      }
      public static void OnExecutedEvent(CommandEventArgs args, ResultInfo result) => ExecutedEvent?.Invoke(args, result);

      #endregion

      #region properties

      public static string ParameterPrefix { get; set; } // if it's null, mapper detects automatically.
      public static int? CommandTimeout { get; set; }
      #endregion

      #region methods

      public static void Use<T>(IParameterHandler handler) where T : new()
      {
         _objectHandlers[typeof(T)] = handler;
      }
      public static void Use<T>(ITypeHandler handler) where T : new()
      {
         _valueHandlers[typeof(T)] = handler;
      }

      #endregion

      #region execute nonquery

      public static int NonQuery(this IDbConnection conn, CommandArgs args, ResultInfo ri)
   => ExecuteNonQueryImpl(conn, args, ri);


      public static int NonQuery(this IDbConnection conn,
         CommandType kind,
         string sql,
         object data = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => NonQuery(conn, new CommandArgs()
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

      #region execute scalar

      public static T Scalar<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri)
         => ExecuteScalarImpl<T>(conn, args, ri);


      public static T Scalar<T>(this IDbConnection conn,
         CommandType kind,
         string sql,
         object data = null,
         int? offset = null,
         int? limit = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => Scalar<T>(conn, new CommandArgs()
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


      public static T Scalar<T>(this IDbConnection conn,
         string sql,
         object data = null,
         int? offset = null,
         int? limit = null,
         int? timeout = null,
         bool? noevent = false,
         object state = null,
         ResultInfo ri = null)

         => Scalar<T>(conn, CommandType.StoredProcedure, sql, data, offset, limit, timeout, noevent, state, ri);

      #endregion

      #region execute query

      public static List<T> Query<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri)
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

         => Query<T>(conn, new CommandArgs()
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


      #endregion

   }
}
