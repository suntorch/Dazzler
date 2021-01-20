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

         var result = connection.Query<TestRecord>(CommandType.Text, "select 'hello Dazzler!'", ri: ri);
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
            Name = "Bill",
            Age = 99
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
