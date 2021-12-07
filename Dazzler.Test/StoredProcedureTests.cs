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
    public sealed class StoredProcedureTests : StoredProcedureTests<SqlServerClientProvider> { }

    public class StoredProcedureTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
    {
        [TestMethod]
        public void SimpleSelect()
        {
            const string create_sp = @"
CREATE OR ALTER PROCEDURE SP_SimpleSelect
	@String varchar(50),
	@Integer int
AS
BEGIN
	select @String String, @Integer Integer
END";


            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, create_sp);

            var args = new
            {
                String = "John",
                Integer = 25
            };

            var result = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_SimpleSelect", args);
            Assert.AreEqual(1, result.Count, "Invalid output record count.");
            Assert.AreEqual(args.Integer, result[0].Integer, "Invalid record.");
        }

        [TestMethod]
        public void RaiseError()
        {
            const string create_sp = @"
CREATE OR ALTER PROCEDURE SP_RaiseError
AS BEGIN
  -- If RAISEERROR is done before the SELECT, an exception will be received in C#
  --RAISERROR('before select', 16, 1);

  SELECT 'Monday' String, 10 Integer
  UNION ALL SELECT 'Tuesday', 20

  -- If RAISEERROR is done after the SELECT, no exception will be received in C#
  RAISERROR('after select', 16, 1);
END";

            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, create_sp);

            var result = connection.Query<ValueTypeTestModel>(CommandType.StoredProcedure, "SP_RaiseError", null);
            Assert.AreEqual(2, result.Count, "Invalid output record count.");
        }
    }
}
