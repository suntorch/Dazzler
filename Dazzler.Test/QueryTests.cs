using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using Dazzler.Models;
using Microsoft.Extensions.Caching.Memory;
using Dazzler.Test.Models;

namespace Dazzler.Test
{
    [TestClass]
    public sealed class QueryTests : QueryTests<SqlServerClientProvider> { }

    public class QueryTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
    {
        [TestMethod]
        public void SimpleSelect()
        {
            ResultInfo ri = new ResultInfo();

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, "select 'Dazzler is cool!' String, 99 Integer, 999.99 Decimal", ri: ri);
            var model = result?.FirstOrDefault();

            Assert.AreEqual(1, result.Count, "Invalid output record count.");
            Assert.AreEqual(1, ri.AffectedRows, "Invalid ResultInfo.AffectedRows.");
        }

        [TestMethod]
        public void MultiRows()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, "select 'Monday' String union all select 'Tuesday'");
            Assert.AreEqual(2, result.Count, "Invalid output record count.");
        }

        [TestMethod]
        public void OffsetLimit()
        {
            string sql = "select Integer from ( values (1),(2),(3),(4),(5),(6),(7) ) as tmp (Integer)";

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, sql, offset: 2, limit: 2);

            Assert.AreEqual(2, result.Count, "Invalid output record count.");
            Assert.AreEqual(3, result[0].Integer, "Fetched wrong record.");
            Assert.AreEqual(4, result[1].Integer, "Fetched wrong record.");
        }

        [TestMethod]
        public void WithInputParameter()
        {
            var args = new
            {
                week = "Monday"
            };

            string sql = "select String from ( values ('Monday'),('Tuesday'),('Wednesday'),('Thursday'),('Monday') ) as tmp (String) where String = @week";

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, sql, args);
            Assert.AreEqual(2, result.Count, "Invalid output record count.");
        }

        [TestMethod]
        public void WithOutputParameter()
        {
            var args = new
            {
                input = "Dazzler is cool!",
                output__out = ""
            };

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"set @output=@input select @input String", args);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(args.input, result.FirstOrDefault().String, "Invalid output resultset.");
            Assert.AreEqual(args.input, args.output__out, "Invalid output parameter value.");
        }

        [TestMethod]
        public void WithCacheOption()
        {
            const string create_sp = @"
CREATE OR ALTER PROCEDURE SP_SelectWithCacheOption
AS
BEGIN
	select format(getdate(), 'yyyy/MM/dd HH:mm:ss.fff') String, 99 Integer
END";


            connection.UseMemoryCache(new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 }));


            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, create_sp);


            var ri = new ResultInfo();
            var cache = new Cache(5);

            // initial getting
            var result = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_SelectWithCacheOption", null, ri: ri, cache: cache)?.FirstOrDefault();
            Assert.AreEqual(1, ri.AffectedRows, "Invalid ResultInfo.AffectedRows.");


            string initialValue = result?.String;

            System.Threading.Thread.Sleep(1000);


            var result2 = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_SelectWithCacheOption", null, ri: ri, cache: cache).FirstOrDefault();
            Assert.AreEqual(1, ri.AffectedRows, "Invalid ResultInfo.AffectedRows.");
            Assert.AreEqual(initialValue, result2?.String, "Invalid cached record.");


            System.Threading.Thread.Sleep(5000);

            var result3 = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_SelectWithCacheOption", null, ri: ri, cache: cache)?.FirstOrDefault();
            Assert.AreEqual(1, ri.AffectedRows, "Invalid ResultInfo.AffectedRows.");
            Assert.AreNotEqual(initialValue, result3?.String, "Cache expiration does not work.");
        }

        [TestMethod]
        public void BindBooleanToIntColumn()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, "select 1 Bool")?.FirstOrDefault();
            Assert.AreEqual(true, result.Bool, "Invalid output Boolean value from 1 (int).");

            result = connection.Query<ValueTypeTestModel>(CommandType.Text, "select 0 Bool")?.FirstOrDefault();
            Assert.AreEqual(false, result.Bool, "Invalid output Boolean value from 0 (int).");
        }

        #region Stored Procedure tests

        const string create_SP_SimpleSelect = @"
CREATE OR ALTER PROCEDURE SP_SimpleSelect
	@String varchar(50),
	@Integer int
AS
BEGIN
	select @String String, @Integer Integer
END";

        [TestMethod]
        public void SP_SimpleSelect()
        {
            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, create_SP_SimpleSelect);

            var args = new
            {
                String = "John",
                Integer = 25
            };

            var result = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_SimpleSelect", args);
            Assert.AreEqual(1, result.Count, "Invalid output record count.");
            Assert.AreEqual(args.Integer, result[0].Integer, "Invalid record.");
        }

        const string create_SP_RaiseError = @"
CREATE OR ALTER PROCEDURE SP_RaiseError
AS BEGIN
  -- If RAISEERROR is done before the SELECT, an exception will be received in C#
  --RAISERROR('before select', 16, 1);

  SELECT 'Monday' String, 10 Integer
  UNION ALL SELECT 'Tuesday', 20

  -- If RAISEERROR is done after the SELECT, no exception will be received in C#
  RAISERROR('after select', 16, 1);
END";

        [TestMethod]
        public void SP_RaiseError()
        {
            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, create_SP_RaiseError);

            var result = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_RaiseError", null);
            Assert.AreEqual(2, result.Count, "Invalid output record count.");
        }

        #endregion

        #region Implementing Events

        const string create_table1 = @"
CREATE TABLE ##DBLog
(
   Started datetime NULL,
   Kind int NULL,
   Sql varchar(4000) NULL,
   Duration int NULL,
   Rows int NULL
)";


        [TestMethod]
        public void WithControlEvent()
        {
            // passes some state data to the events to control operation.
            var state = new ExecuteEventState();

            // let's stop any non-query operation such as update, insert, delete.
            state.StopNonQuery = true;

            // creates temp table at first.
            connection.NonQuery(CommandType.Text, create_table1, state: state);


            Mapper.ExecutingEvent += Mapper_ExecutingEvent;
            Mapper.ExecutedEvent += Mapper_ExecutedEvent;

            // somewhere non-query execution is called.
            SimpleSelect();

            Mapper.ExecutingEvent -= Mapper_ExecutingEvent;
            Mapper.ExecutedEvent -= Mapper_ExecutedEvent;
        }

        private void Mapper_ExecutedEvent(CommandEventArgs args, ResultInfo result)
        {
            var param = new
            {
                Started = DateTime.Now,
                Kind = args.Kind,
                args.Sql,
                result.Duration,
                Rows = result.AffectedRows
            };

            // ATTENTION: Any database operation in this event function should not trigger events!
            // Otherwise, it will cause recursive call for the event function and it will never end.

            var affectedRows = connection.NonQuery(CommandType.Text
               , "insert into ##DBLog (Started,Kind,Sql,Duration,Rows) values (@Started,@Kind,@Sql,@Duration,@Rows)"
               , param
               , noevent: true);

            Assert.AreEqual(1, affectedRows, "Invalid inserted log record.");

        }

        private void Mapper_ExecutingEvent(CommandEventArgs args)
        {
            // the event function will be invoked when a command is coming to execute.
            Console.WriteLine("Executing {0}: {1}", args.Kind, args.Sql);

            // also, you are able to CANCEL the execution based on your state information.
            if (args.State == null) return;
            if (args.State is ExecuteEventState myControl)
            {
                if (myControl.StopNonQuery && args.ExecutionType == ExecutionType.NonQuery)
                    args.Cancel = true;
            }
        }

        #endregion
    }
}
