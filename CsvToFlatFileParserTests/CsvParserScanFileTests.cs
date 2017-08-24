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
    public class CsvParserScanFileTests
    {
        /// <summary>
        /// Parses if it is valid file
        /// currently allowing file to parse if there are any missing columns in a row by skipping that row.
        /// </summary>
        [TestMethod()]
        public void ScanFileTest_Positive()
        {

            CsvParser parser = new CsvParser();
            string filepath = @"C:\temp\unittest\test.csv";
            try
            {
                bool result = parser.ScanFile(filepath);
                Assert.IsTrue(result);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// checks if file exists in the path and is not empty file
        /// </summary>
        [TestMethod()]
        public void ScanFileTest_Negative()
        {

            CsvParser parser = new CsvParser();
            bool Failureresult = false;
            //checks if file exists in specified location
            string filepath = @"C:\temp\unittest\test1.csv";
            try
            {
                parser.ScanFile(filepath);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Unable to find the specified file.");
            }

            //Checks if file is empty file
            filepath = @"C:\temp\unittest\test_empty.csv";
            try
            {
                parser.ScanFile(filepath);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Empty File.Terminating.");
            }
        }

        /// <summary>
        ///  File parsing fails if no.of columns ( >6).Hence Terminating..
        /// </summary>
        [TestMethod()]
        public void ScanFileTest_ExceptionCase()
        {

            CsvParser parser = new CsvParser();
            string filepath = @"C:\temp\unittest\test_invalidcolumns.csv";
            try
            {
                parser.ScanFile(filepath);
            }
            catch (Exception e)
            {
                bool result = e.Message.Contains("Invalid File format. Number of columns (>6).Hence Terminating file parsing..");
                Assert.AreEqual(true, result);

            }

        }

      }
}