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
    public class CsvParserIsValidSINTests
    {
        /// <summary>
        /// validates if SIN is in valid format or not.Vaild Format is xxx-xx-xxxx
        /// </summary>
        [TestMethod()]
        public void IsValidSINTest_Positive()
        {
            CsvParser parser = new CsvParser();
            string SIN = "124-21-2345";
            bool result = parser.IsValidSIN(SIN);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Invalid SIN formats are validated.Vaild Format is xxx-xx-xxxx
        /// </summary>
        [TestMethod()]
        public void IsValidSINTest_Negative()
        {
            CsvParser parser = new CsvParser();
            bool result = false;
            string SIN = "124-210-234";
            result = parser.IsValidSIN(SIN);
            Assert.IsFalse(result);

            SIN = "124-21-234k";
            result = parser.IsValidSIN(SIN);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Invalid SIN formats are validated.Vaild Format is xxx-xx-xxxx
        /// </summary>
        [TestMethod()]
        public void IsValidSINTest_AlphaNumeric()
        {
            CsvParser parser = new CsvParser();
            bool result = false;

            string SIN = "124-21-234k";
            result = parser.IsValidSIN(SIN);
            Assert.IsFalse(result);
        }

        /// <summary>
        ///  validate if SIN is of vaild length
        /// </summary>
        [TestMethod()]
        public void IsValidSINTest_InvalidLength()
        {
            CsvParser parser = new CsvParser();
            string SIN = "124-2100-234";
            try
            {
                parser.IsValidSIN(SIN);
            }
            catch (Exception e)
            {

                Assert.AreEqual(e.Message, "Invalid SIN");
            }

        }

        /// <summary>
        ///  Check if SIN doesn't contain any space
        /// </summary>
        [TestMethod()]
        public void IsValidSINTest_ExceptionCase()
        {
            CsvParser parser = new CsvParser();

            string SIN = "124-21-2345 ";
            try
            {
                parser.IsValidSIN(SIN);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid SIN");
            }
        }
    }
}