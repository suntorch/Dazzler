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
    public sealed class OutputIdentityTests : OutputIdentityTests<SqlServerClientProvider> { }

    public class OutputIdentityTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
    {
        const string drop_DazzlerIdentityTable = @"DROP TABLE IF EXISTS ##DazzlerIdentityTable";
        const string create_DazzlerIdentityTable = @"
CREATE TABLE ##DazzlerIdentityTable
(
   Id bigint IDENTITY(852,3) NOT NULL,
   Value varchar(100) NULL
)";
        const string create_DazzlerIdentityInsert = @"
CREATE OR ALTER PROCEDURE DazzlerIdentityInsert
   @Id bigint out,
   @Value varchar(100)
AS
BEGIN
   insert into ##DazzlerIdentityTable (Value) values (@Value)
   set @Id = @@IDENTITY
END";

        [TestMethod]
        public void InsertIdentityBySuffix()
        {
            // create a test stored procedure.
            connection.NonQuery(CommandType.Text, drop_DazzlerIdentityTable);
            connection.NonQuery(CommandType.Text, create_DazzlerIdentityTable);
            connection.NonQuery(CommandType.Text, create_DazzlerIdentityInsert);

            var args = new
            {
                Id__inout = 0,
                Value = "Text value"
            };

            // insert first row.
            var result = connection.NonQuery(CommandType.StoredProcedure, "DazzlerIdentityInsert", args);
            Assert.AreEqual(852, args.Id__inout, "Invalid returning identity value.");

            // insert second row.
            result = connection.NonQuery(CommandType.StoredProcedure, "DazzlerIdentityInsert", args);
            Assert.AreEqual(855, args.Id__inout, "Invalid returning identity value.");

        }
    }
}
