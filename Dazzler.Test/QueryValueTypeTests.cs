using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using Dazzler.Models;
using Dazzler.Test.Models;

namespace Dazzler.Test
{
    [TestClass]
    public sealed class QueryValueTypeTests : QueryValueTypeTests<SqlServerClientProvider> { }

    public class QueryValueTypeTests<TProvider> : TestBase<TProvider> where TProvider : DatabaseProvider
    {
        [TestMethod]
        public void StringValue()
        {
            ResultInfo ri = new ResultInfo();

            string value = "hello Dazzler!";

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select '{value}' String", ri: ri);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(value, result.FirstOrDefault()?.String, "Invalid value.");
        }

        [TestMethod]
        public void DateTimeValue()
        {
            DateTime value = new DateTime(2055, 12, 31, 23, 59, 59);

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select '{value}' DateTime", null);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(value, result.FirstOrDefault()?.DateTime, "Invalid value.");
        }

        [TestMethod]
        public void IntValue()
        {
            int value = int.MaxValue;

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select {value} Integer", null);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(value, result.FirstOrDefault()?.Integer, "Invalid value.");
        }

        [TestMethod]
        public void DecimalValue()
        {
            decimal value = decimal.MaxValue;

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select {value} Decimal", null);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(value, result.FirstOrDefault()?.Decimal, "Invalid value.");
        }

        [TestMethod]
        public void DoubleValue()
        {
            double value = double.MaxValue;

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select {value} [Double]", null);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreEqual(value, result.FirstOrDefault()?.Double, "Invalid value.");
        }

        [TestMethod]
        public void GuidValue()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select NEWID() Guid", null);
            Assert.AreEqual(1, result.Count, "Invalid row.");
            Assert.AreNotEqual(Guid.Empty, result.FirstOrDefault()?.Guid, "Invalid value.");
        }

        [TestMethod]
        public void EnumValue()
        {
            var args = new
            {
                inputValue = System.Data.ConnectionState.Executing,
                outputValue__out = (System.Data.ConnectionState?)null
            };

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select @outputValue=@inputValue", args);
            Assert.AreEqual(args.inputValue, args.outputValue__out, "Invalid value.");
        }

        [TestMethod]
        public void EnumNullValue()
        {
            var args = new
            {
                inputValue = (System.Data.ConnectionState?)null,
                outputValue__out = (System.Data.ConnectionState?)null
            };

            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select @outputValue=@inputValue", args);
            Assert.AreEqual(args.inputValue, args.outputValue__out, "Invalid value.");
        }

        [TestMethod]
        public void BoolValueFromInt()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select 1 Bool")?.FirstOrDefault();
            Assert.AreEqual(true, result.Bool, "Invalid value for True.");

            var result2 = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select 0 Bool")?.FirstOrDefault();
            Assert.AreEqual(false, result2.Bool, "Invalid value for False.");
        }

        [TestMethod]
        public void BoolValueFromNumericString()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select '1' Bool")?.FirstOrDefault();
            Assert.AreEqual(true, result.Bool, "Invalid value for True.");

            var result2 = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select '0' Bool")?.FirstOrDefault();
            Assert.AreEqual(false, result2.Bool, "Invalid value for False.");
        }

        [TestMethod]
        public void BoolValueFromString()
        {
            var result = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select 'True' Bool")?.FirstOrDefault();
            Assert.AreEqual(true, result.Bool, "Invalid value for True.");

            var result2 = connection.Query<ValueTypeTestModel>(CommandType.Text, $"select 'False' Bool")?.FirstOrDefault();
            Assert.AreEqual(false, result2.Bool, "Invalid value for False.");
        }
    }
}
