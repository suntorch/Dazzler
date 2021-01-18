using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Peppy.Interfaces;


namespace Peppy.Handlers
{
   public class DataAdapterReader
   {
      public DataSet ReadDatSet(IDbCommand command, ExecuteArgs args, ResultInfo ri)
      {
         DataSet ds = new DataSet();

         //@@@amar20180705: Latest version of MySql data driver returns NULL object for CreateDataAdapter!
         //@@@amar20210113: This MySql bug is fixed on Apr-2019 in MySQL Connector/NET 8.0.13 and 6.10.9 release.

         using (var adapter = DbProviderFactories.GetFactory((DbConnection)command.Connection).CreateDataAdapter())
         {
            adapter.SelectCommand = (DbCommand)command;

            if (args.Offset.HasValue && args.Limit.HasValue)
               ri.AffectedRows = adapter.Fill(ds, args.Offset.Value, args.Limit.Value, null);
            else
               ri.AffectedRows = adapter.Fill(ds);
         }
         return ds;
      }

      public DataTable ReadDataTable(IDbCommand command, ExecuteArgs args, ResultInfo ri)
      {
         DataTable dt = new DataTable();
         using (var adapter = DbProviderFactories.GetFactory((DbConnection)command.Connection).CreateDataAdapter())
         {
            adapter.SelectCommand = (DbCommand)command;

            if (args.Offset.HasValue && args.Limit.HasValue)
               ri.AffectedRows = adapter.Fill(args.Offset.Value, args.Limit.Value, dt);
            else
               ri.AffectedRows = adapter.Fill(dt);
         }
         return dt;
      }

   }
}
