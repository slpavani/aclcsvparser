using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsvToFlatFileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvToFlatFileParser.Tests
{
    [TestClass()]
    public class CsvParserIsNumericTypeTests
    {
        /// <summary>
        /// checks if the column value is string or numeric type.Valid format : 12.1, 123, 1234.21
        /// </summary>
        [TestMethod()]
        public void IsNumericTypeTest_Positive()
        {
            CsvParser parser = new CsvParser();
            bool result = false;
            const string field = "12";
            result = parser.IsNumericType(field);
            Assert.IsTrue(result);

            const string fieldWithOneDecimal = "128.1";
            result = parser.IsNumericType(fieldWithOneDecimal);
            Assert.IsTrue(result);

            const string fieldWithTwoDecimal = "125.12";
            result = parser.IsNumericType(fieldWithTwoDecimal);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// checks for invalid column value types
        /// </summary>
        [TestMethod()]
        public void IsNumericTypeTest_Negative()
        {
            CsvParser parser = new CsvParser();
            bool result = false;

            //field type is string
            const string field = "Test";
            result = parser.IsNumericType(field);
            Assert.IsFalse(result);

            //field has more values after decimal
            const string fieldWithOneDecimal = "128.123";
            result = parser.IsNumericType(fieldWithOneDecimal);
            Assert.IsFalse(result);

            //field is aplhanumeric
            const string fieldWithTwoDecimal = "125c.12";
            result = parser.IsNumericType(fieldWithTwoDecimal);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// checks if the column value has more than two decimals
        /// </summary>
        [TestMethod()]
        public void IsNumericTypeTest_ExceptionCase()
        {
            CsvParser parser = new CsvParser();
            bool result = false;
            const string field = "12.23.9";
            result = parser.IsNumericType(field);
            Assert.IsFalse(result);

       }
    }
}