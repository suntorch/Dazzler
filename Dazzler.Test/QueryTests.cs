using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using Dazzler.Models;


namespace Dazzler.Test
{
   [TestClass]
   public sealed class QueryTests : QueryTests<SqlServerClientProvider> { }

   public class QueryTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
   {
      #region Select Query tests

      [TestMethod]
      public void SelectQuery_Basic()
      {
         ResultInfo ri = new ResultInfo();

         var result = connection.Query<TestRecord>(CommandType.Text, "select 'hello Dazzler!' name", ri: ri);
         Assert.AreEqual(1, result.Count, "Invalid output record count.");
         Assert.AreEqual(1, ri.AffectedRows, "Invalid ResultInfo.AffectedRows.");
      }


      [TestMethod]
      public void SelectQuery_MultiRows()
      {
         var result = connection.Query<TestRecord>(CommandType.Text, "select 'John' Name union all select 'Doe'");
         Assert.AreEqual(2, result.Count, "Invalid output record count.");
      }


      [TestMethod]
      public void SelectQuery_MultiRows_OffsetLimit()
      {
         string sql = "select Age from ( values (1),(2),(3),(4),(5),(6),(7) ) as tmp (Age)";

         var result = connection.Query<TestRecord>(CommandType.Text, sql, offset: 2, limit: 2);

         Assert.AreEqual(2, result.Count, "Invalid output record count.");
         Assert.AreEqual(3, result[0].Age, "Fetched wrong record.");
         Assert.AreEqual(4, result[1].Age, "Fetched wrong record.");
      }

      [TestMethod]
      public void SelectQuery_WithParameters()
      {
         var args = new
         {
            Name = "John",

         };

         var result = connection.Query<TestRecord>(CommandType.Text, "select * from MyTable where Name = @Name");
         Assert.AreEqual(2, result.Count, "Invalid output record count.");
      }

      #endregion

      #region Stored Procedure tests

      string create_proc1 = @"
CREATE OR ALTER PROCEDURE DazzlerProc1
	@Name varchar(50),
	@Age int
AS
BEGIN
	select @Name Name, @Age Age
END";

      [TestMethod]
      public void SelectQuery_SP()
      {
         // create a test stored procedure.
         connection.NonQuery(CommandType.Text, create_proc1);

         var args = new
         {
            Name = "John",
            Age = 25
         };

         var result = connection.Query<TestRecord>(CommandType.StoredProcedure, "DazzlerProc1", args);
         Assert.AreEqual(1, result.Count, "Invalid output record count.");
         Assert.AreEqual(99, result[0].Age, "Fetched wrong record.");

      }

      string create_proc2 = @"
CREATE OR ALTER PROCEDURE DazzlerProc2
AS BEGIN
  -- If RAISEERROR is done before the SELECT, an exception will be received in C#
  --RAISERROR('before select', 16, 1);

  SELECT 'John' Name, 10 Age
  UNION ALL SELECT 'Bob', 20

  -- If RAISEERROR is done after the SELECT, no exception will be received in C#
  RAISERROR('after select', 16, 1);
END";

      [TestMethod]
      public void SelectQuery_SP_with_RaiseError()
      {
         // create a test stored procedure.
         connection.NonQuery(CommandType.Text, create_proc2);

         var result = connection.Query<TestRecord>(CommandType.StoredProcedure, "DazzlerProc2", null);
         Assert.AreEqual(2, result.Count, "Invalid output record count.");
      }

      #endregion

      #region Implementing Events

      [TestMethod]
      public void SelectQuery_WithEvent()
      {
         Mapper.ExecutingEvent += Mapper_ExecutingEvent;
         Mapper.ExecutedEvent += Mapper_ExecutedEvent;

         SelectQuery_Basic();

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
            , "insert into DBLog (Started,Kind,Sql,Duration,Rows) values (@Started,@Kind,@Sql,@Duration,@Rows)"
            , param
            , noevent: true);

         Assert.AreEqual(1, affectedRows, "Invalid inserted log record.");

      }

      private void Mapper_ExecutingEvent(CommandEventArgs args)
      {
         Console.WriteLine("Executing {0}: {1}", args.Kind, args.Sql);
      }

      #endregion
   }


   #region model classes
   public class TestRecord
   {
      public string Name { get; set; }
      public int Age { get; set; }
      public DateTime Dob { get; set; }
      public decimal Money { get; set; }
   }

   #endregion
}
