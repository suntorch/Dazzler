using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using Dazzler.Models;
using Dazzler.Test.Models;

namespace Dazzler.Test
{
    [TestClass]
    public sealed class OutputParameterTests : OutputParameterTests<SqlServerClientProvider> { }

    public class OutputParameterTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
    {
        [TestMethod]
        public void BindStronglyTypedModel()
        {
            var args = new OutputTestModel()
            {
                input = "Dazzler is cool!",
                output = ""
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output, "Invalid output value.");
        }

        [TestMethod]
        public void BindString()
        {
            var args = new
            {
                input = "Dazzler is cool!",
                output__out = ""
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindDateTime()
        {
            var args = new
            {
                input = new DateTime(2021, 12, 31, 23, 59, 59),
                output__out = (DateTime?)null
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindInt()
        {
            var args = new
            {
                input = int.MaxValue,
                output__out = 0
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindDecimal()
        {
            var args = new
            {
                input = decimal.MaxValue,
                output__out = 0M
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindDouble()
        {
            var args = new
            {
                input = double.MaxValue,
                output__out = 0D
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindGuid()
        {
            var args = new
            {
                input = Guid.NewGuid(),
                output__out = (Guid?)null
            };

            var result = connection.NonQuery(CommandType.Text, $"set @output=@input", args);
            Assert.AreEqual(args.input, args.output__out, "Invalid output value.");
        }

        [TestMethod]
        public void BindNullable()
        {
            var args = new
            {
                String = (string)null,
                Date = (DateTime?)null,
                Integer = (int?)null,

                ResultString__out = "",
                ResultDate__out = "",
                ResultInteger__out = "",
            };

            connection.NonQuery(CommandType.Text, @"
set @ResultString=isnull(@String,'OK') 
set @ResultDate=iif(@Date is null,'OK','') 
set @ResultInteger=iif(@Integer is null,'OK','')", args);

            Assert.AreEqual("OK", args.ResultString__out, "Invalid null string.");
            Assert.AreEqual("OK", args.ResultDate__out, "Invalid null datetime.");
            Assert.AreEqual("OK", args.ResultInteger__out, "Invalid null integer.");
        }

    }
}
