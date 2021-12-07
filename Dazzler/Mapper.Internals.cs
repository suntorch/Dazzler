using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq;
using Dazzler.Models;
using Dazzler.Interfaces;
using Dazzler.Handlers;
using Dazzler.Readers;

namespace Dazzler
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
      /// The parameter name may contain the following suffixes "Name[__in|out|inout|ret][9*]"
      /// 
      /// [__in|out|inout|ret] ==> specifies a direction of the parameter.
      /// following number [9*] ==> specifies a value size of the parameter.
      /// 
      /// For example:
      /// FirstName__out    ==> means, parameter name is FirstName, direction is Output
      /// LastName__ret16   ==> means, parameter name is LastName, direction is ReturnValue and Size is 16
      /// 
      /// </summary>
      /// <param name="command"></param>
      /// <param name="name"></param>
      /// <param name="value"></param>
      /// <param name="type"></param>
      private static void AssignParameter(IDbDataParameter parameter, string name, object value, Type type, BindAttribute attr, out ITypeHandler typeHandler)
      {
         ParameterDirection direction = ParameterDirection.Input;
         string actualName = name;
         int size = 0;

         // if attribute is specified it ignores suffixes!
         if (attr != null)
         {
            direction = attr.Direction;
            size = attr.Size;
         }
         else
         {
            // extracts a suffix if it has.
            ParsePropertyName(name, out actualName, out direction, out size);
         }

         // assigns parameter values.
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

      private static void BuildDatabaseParameters(IDbCommand command, object parameterObject, out List<OutputParameterInfo> outputs)
      {
         outputs = null;
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

         List<OutputParameterInfo> opi = new List<OutputParameterInfo>();

         // declares action method to handle database parameter. It will be called for each member.
         ParameterOptions options = (name, value, type, attr) =>
            {
               // creates parameter object.
               IDbDataParameter p = command.CreateParameter();

               // assigns parameter input value with parsing suffixes.
               AssignParameter(p, name, value, type, attr, out ITypeHandler typeHandler);

               // adds database parameter.
               command.Parameters.Add(p);

               // save information to assign output parameter values.
               if (p.Direction != ParameterDirection.Input)
               {
                  // typeHandler will be used to convert output database value back to the property.
                  opi.Add(new OutputParameterInfo
                  {
                     parameterName = p.ParameterName,
                     parameterHandler = parameterHandler,
                     propertyName = name,
                     propertyType = type,
                     typeHandler = typeHandler
                  });
               }
            };


         // extracts parameter object members.
         parameterHandler.ForEach(parameterObject, options);

         // return for next operation.
         outputs = opi;
      }
      private static void ReadOutputParameters(IDbCommand command, object parameterObject, List<OutputParameterInfo> outputs)
      {
         if (parameterObject == null || outputs == null) return;

         foreach (var opi in outputs)
         {
            if (command != null)
            {
               // converts database output value to the property type.
               IDbDataParameter p = (IDbDataParameter)command.Parameters[opi.parameterName];
               opi.value = opi.typeHandler.Parse(p.Value, opi.propertyType);
            }

            // sets the value to the property.
            opi.parameterHandler.SetValue(parameterObject, opi.propertyName, opi.value);
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
         args.ExecutionType = ExecutionType.NonQuery;

         return ExecuteImpl<int>(conn, args, ri, (command, data) =>
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
         args.ExecutionType = ExecutionType.Scalar;

         return ExecuteImpl<T>(conn, args, ri, (command, data) =>
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
      private static List<T> ExecuteQueryImpl<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri, QueryGenericReader<T> reader)
      {
         return ExecuteImpl<T>(conn, args, ri, (command, data) => reader?.Read(command, data));
      }


      /// <summary>
      /// Generalized a query execution method.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="conn"></param>
      /// <param name="args"></param>
      /// <param name="readerAction"></param>
      /// <returns></returns>
      private static List<T> ExecuteImpl<T>(this IDbConnection conn, CommandArgs args, ResultInfo ri, Func<IDbCommand, CommandArgs, List<T>> readerAction)
      {
         // validates some input parameters.
         if (args?.Sql == null) throw new ArgumentNullException("Sql");

         #region initialize variables
         List<T> result = null;
         long elapsed = 0;
         bool cacheExists = false;
         List<OutputParameterInfo> outputs = null;
         #endregion

         #region invoke the event Executing.
         var eventArgs = Utility.Copy<CommandEventArgs>(args);
         if (!(args.NoEvent ?? false)) OnExecutingEvent(eventArgs);
         #endregion

         #region check cache options
         var cacheManager = new CacheManager(args);
         cacheExists = cacheManager.Get(out CacheItem ci);
         if (cacheExists)
         {
            // assign output parameter values from the cache.
            ReadOutputParameters(null, args.Data, ci.outputs);

            // assign ouput result from the cache.
            result = (List<T>)ci.result;

            // skip database access and jump to the final stage.
            goto OnFetchCompleted;
         }
         #endregion


         // execute the command.
         using (DbCommand command = (DbCommand)conn.CreateCommand())
         {
            // builds database parameter.
            BuildDatabaseParameters(command, args.Data, out outputs);

            // passes some inputs.
            command.Transaction = (DbTransaction)args.Transaction;
            command.CommandTimeout = args.Timeout ?? CommandTimeout ?? 0; // in seconds
            command.CommandText = args.Sql;
            command.CommandType = args.Kind;

            bool wasClosed = conn.State == ConnectionState.Closed;
            var stopwatch = Stopwatch.StartNew();

            // invokes an action to handle database result.
            try
            {
               if (wasClosed) conn.Open();
               result = readerAction?.Invoke(command, args);
            }
            finally { if (wasClosed) conn.Close(); }

            // measures some performance.
            stopwatch.Stop();
            elapsed = stopwatch.ElapsedMilliseconds;

            // reads output parameters.
            ReadOutputParameters(command, args.Data, outputs);
         }


      OnFetchCompleted:

         if (ri != null)
         {
            ri.Rows = result;
            ri.Duration = elapsed;
            ri.FromCache = cacheExists;
            ri.AffectedRows = result?.Count ?? 0;
         }

         // add the result into the cache if it's cacheable.
         if (!cacheExists) cacheManager.Put(new CacheItem { result = result, outputs = outputs });


         if (!(args.NoEvent ?? false)) OnExecutedEvent(eventArgs, ri);

         return result;
      }



      #endregion

   }
}
