﻿using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using System.Dynamic;
using Peppy.Models;
using Peppy.Interfaces;
using Peppy.Handlers;
using Peppy.Readers;

namespace Peppy
{
   public static partial class Mapper
   {

      #region internal - variables
      internal static Dictionary<Type, IParameterHandler> _objectHandlers = new Dictionary<Type, IParameterHandler>();
      internal static Dictionary<Type, ITypeHandler> _valueHandlers = new Dictionary<Type, ITypeHandler>();
      internal static Dictionary<Type, DbType> _typeMap = new Dictionary<Type, DbType>()
      {
         {typeof(byte), DbType.Byte},
         {typeof(sbyte), DbType.SByte},
         {typeof(short), DbType.Int16},
         {typeof(ushort), DbType.UInt16},
         {typeof(int), DbType.Int32},
         {typeof(uint), DbType.UInt32},
         {typeof(long), DbType.Int64},
         {typeof(ulong), DbType.UInt64},
         {typeof(float), DbType.Single},
         {typeof(double), DbType.Double},
         {typeof(decimal), DbType.Decimal},
         {typeof(bool), DbType.Boolean},
         {typeof(string), DbType.String},
         {typeof(char), DbType.StringFixedLength},
         {typeof(Guid), DbType.Guid},
         {typeof(DateTime), DbType.DateTime},
         {typeof(DateTimeOffset), DbType.DateTimeOffset},
         {typeof(TimeSpan), DbType.Time},
         {typeof(byte[]), DbType.Binary},
         {typeof(byte?), DbType.Byte},
         {typeof(sbyte?), DbType.SByte},
         {typeof(short?), DbType.Int16},
         {typeof(ushort?), DbType.UInt16},
         {typeof(int?), DbType.Int32},
         {typeof(uint?), DbType.UInt32},
         {typeof(long?), DbType.Int64},
         {typeof(ulong?), DbType.UInt64},
         {typeof(float?), DbType.Single},
         {typeof(double?), DbType.Double},
         {typeof(decimal?), DbType.Decimal},
         {typeof(bool?), DbType.Boolean},
         {typeof(char?), DbType.StringFixedLength},
         {typeof(Guid?), DbType.Guid},
         {typeof(DateTime?), DbType.DateTime},
         {typeof(DateTimeOffset?), DbType.DateTimeOffset},
         {typeof(TimeSpan?), DbType.Time},
         {typeof(object), DbType.Object}
      };
      internal static string _prefix = null;
      #endregion

      #region internal - type handlers

      private static bool ParsePropertyName(string text, out string name, out ParameterDirection direction, out int size)
      {
         name = text; size = 0; direction = ParameterDirection.Input;
         Match m = Regex.Match(text, "^(.+)__(in|out|inout|ret)([0-9]*)$", RegexOptions.IgnoreCase);
         if (m.Success)
         {
            name = m.Groups[1].Value;
            string dir = m.Groups[2].Value;
            if (m.Groups.Count > 3) size = Utility.ToInt(m.Groups[3].Value);

            if (!string.IsNullOrEmpty(dir))
            {
               switch (dir.ToLower())
               {
                  case "in": direction = ParameterDirection.Input; break;
                  case "out": direction = ParameterDirection.Output; break;
                  case "inout": direction = ParameterDirection.InputOutput; break;
                  case "ret": direction = ParameterDirection.ReturnValue; break;
               }
            }
         }
         return m.Success;
      }


      /// <summary>
      /// if parameter name ends with "__in|out|inout|ret"
      /// then it mentions direction of the parameter.
      /// So name needs to be extracted to actual name
      /// and direction type.
      /// 
      /// If you want to specify value Size for output parameter
      /// then you can put Numeric value at the end.
      /// 
      /// Example:
      /// FirstName__out    ==> parameter name is FirstName, direction is Output
      /// LastName__ret16   ==> it means, name is LastName, direction is ReturnValue and Size is 16
      /// 
      /// </summary>
      /// <param name="command"></param>
      /// <param name="name"></param>
      /// <param name="value"></param>
      /// <param name="type"></param>
      private static void AssignParameter(IDbDataParameter parameter, string name, object value, Type type, out ITypeHandler typeHandler)
      {
         // extracts a suffix if it has.
         ParsePropertyName(name, out string actualName, out ParameterDirection direction, out int size);

         // general.
         parameter.ParameterName = $"{_prefix}{actualName}";
         parameter.Direction = direction;
         parameter.Size = size;

         // rethink the size for output string parameter!
         if (direction != ParameterDirection.Input && size == 0 && type == typeof(string)) parameter.Size = 8000;


         // finds value handler.
         _valueHandlers.TryGetValue(type, out typeHandler);

         // to be safe
         if (typeHandler == null) typeHandler = new ValueTypeHandler();

         // assigns parameter value.
         typeHandler.SetValue(parameter, value, type);
      }

      private static void BuildDatabaseParameters(IDbCommand command, dynamic parameterObject, out ParameterLookup lookup)
      {
         lookup = null;
         if (parameterObject == null) return;

         #region database specific operations.
         Utility.SetPropertyValue(command, "BindByName", true); // specific for Oracle.
         DeterminePrefix(command.Connection);
         #endregion

         #region finds parameter object handler.
         Type objectType = parameterObject?.GetType() ?? typeof(object);
         _objectHandlers.TryGetValue(objectType, out IParameterHandler parameterHandler);

         // picks default handler if no matching handler.
         if (parameterHandler == null) parameterHandler = new ObjectParameterHandler();
         #endregion


         ParameterLookup opl = new ParameterLookup()
         {
            map = new Dictionary<IDbDataParameter, ParameterLookupData>(),
            parameterHandler = parameterHandler
         };

         // declares action method to handle parameter. It will be called for each member.
         Action<string, object, Type> action = delegate (string propertyName, object propertyValue, Type propertyType)
         {
            IDbDataParameter p = command.CreateParameter();
            AssignParameter(p, propertyName, propertyValue, propertyType, out ITypeHandler typeHandler);
            command.Parameters.Add(p);

            // save information to assign output parameter values.
            if (p.Direction != ParameterDirection.Input)
            {
               // typeHandler will be used to convert output database value back to the property.
               opl.map[p] = new ParameterLookupData { name = propertyName, type = propertyType, typeHandler = typeHandler };
            }
         };


         // extracts parameter object members.
         parameterHandler.ForEach(parameterObject, action);

         // return for next operation.
         lookup = opl;
      }
      private static void ReadOutputParameters(IDbCommand command, dynamic parameterObject, ParameterLookup lookup)
      {
         if (parameterObject == null) return;


         // reads output parameters to back to object.
         foreach (IDbDataParameter data in command.Parameters)
         {
            if (data.Direction == ParameterDirection.Input) continue;

            ParameterLookupData mapInfo = null;
            lookup.map.TryGetValue(data, out mapInfo);
            if (mapInfo?.name == null) continue; // should not be possible, anyways skip it.


            // assigns parameter value.
            object value = mapInfo.typeHandler.Parse(data.Value, mapInfo.type);

            // back to the property value from database value.
            lookup.parameterHandler.SetValue(parameterObject, mapInfo.name, value);
         }
      }

      #endregion

      #region internal - database specific functions
      private static void DeterminePrefix(IDbConnection conn)
      {
         // it needs to be determined one time.
         if (_prefix != null) return;

         //_prefix = ParameterPrefix != null
         //   ? "" + (ParameterPrefix + "@")[0]
         //   : ((DbConnection)conn)
         //      .GetSchema(DbMetaDataCollectionNames.DataSourceInformation)
         //      .Rows[0][DbMetaDataColumnNames.ParameterMarkerFormat].ToString();


         //DataSourceInformation dsi = new DataSourceInformation(conn.GetSchema(DbMetaDataCollectionNames.DataSourceInformation));
         //return new string(dsi.ParameterMarkerPattern[0]);
      }
      #endregion

      #region static constructor

      static Mapper()
      {
         // parameter object handler
         Use<object>(new ObjectParameterHandler()); // default handler.
         Use<ExpandoObject>(new ExpandoParameterHanlder());
         Use<Dictionary<string, object>>(new DictionaryParameterHandler());

         // value type handler
         Use<object>(new ValueTypeHandler());  // default value type handler.
         Use<DataTable>(new DataTableValueHandler());
      }

      #endregion

      #region internal - core executors


      /// <summary>
      /// Executes non-query command and returns number of rows affected.
      /// </summary>
      /// <param name="conn"></param>
      /// <param name="args"></param>
      /// <returns></returns>
      private static int ExecuteNonQueryImpl(this IDbConnection conn, CommandArgs args, ResultInfo ri)
      {
         return ExecuteImpl<int>(conn, args, ri, (command, args, lookup) =>
         {
            List<int> result = new List<int>();
            result.Add(command.ExecuteNonQuery());
            return result;
         }).FirstOrDefault();
      }


      /// <summary>
      /// Executes the query and returns only the first column of the first row in the resultset returned by the query.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="conn"></param>
      /// <param name="args"></param>
      /// <returns></returns>
      private static T ExecuteScalarImpl<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri)
      {
         return ExecuteImpl<T>(conn, args, ri, (command, args, lookup) =>
         {
            List<T> result = new List<T>();
            result.Add((T)new ValueTypeHandler().Parse(command.ExecuteScalar(), typeof(T)));
            return result;
         }).FirstOrDefault();
      }


      /// <summary>
      /// Executes the query and returns resultset returned by the query.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="conn"></param>
      /// <param name="args"></param>
      /// <param name="reader"></param>
      /// <returns></returns>
      private static List<T> ExecuteImpl<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri, QueryGenericReader<T> reader)
      {
         return ExecuteImpl<T>(conn, args, ri, (command, args, lookup) => reader?.Read(command, args));
      }


      /// <summary>
      /// Generalized a query execution method.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="conn"></param>
      /// <param name="args"></param>
      /// <param name="readerAction"></param>
      /// <returns></returns>
      private static List<T> ExecuteImpl<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri, Func<IDbCommand, CommandArgs, ParameterLookup, List<T>> readerAction)
      {
         List<T> result = null;

         // validates some input parameters.
         if (args?.Sql == null) throw new ArgumentNullException("Sql");


         CommandEventArgs eventArgs = new CommandEventArgs();
         Utility.Copy(args, eventArgs);


         // invoke the event Executing.
         if (!(args.NoEvent ?? false)) OnExecutingEvent(eventArgs);


         using (DbCommand command = (DbCommand)conn.CreateCommand())
         {
            // builds database parameter.
            BuildDatabaseParameters(command, args.Data, out ParameterLookup lookup);

            // passes some inputs.
            command.Transaction = (DbTransaction)args.Transaction;
            command.CommandTimeout = args.Timeout ?? CommandTimeout ?? 0; // in seconds
            command.CommandText = args.Sql;
            command.CommandType = args.Kind;

            var stopwatch = Stopwatch.StartNew();

            // invokes an action to handle databas result.
            result = readerAction?.Invoke(command, args, lookup);

            // measures some performance.
            stopwatch.Stop();
            if (ri != null)
            {
               ri.Rows = result;
               ri.Duration = stopwatch.ElapsedMilliseconds;
               ri.AffectedRows = result?.Count ?? 0;
            }


            // reads output parameters.
            ReadOutputParameters(command, args.Data, lookup);
         }

         if (!(args.NoEvent ?? false)) OnExecutedEvent(eventArgs, ri);

         return result;
      }


      #endregion



   }
}
