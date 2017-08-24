using System;
using System.Runtime.Remoting.Messaging;

namespace CsvToFlatFileParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string path = @"C:\temp\unittest\test_invalidcolumns.csv";
            CsvParser parser = new CsvParser();
            bool success = false;
            try
            {
               success = parser.ScanFile(path);
               Console.WriteLine("File Parsing successful"+ success);
            }
            catch (Exception e)
            {
              Console.WriteLine("File Parsing fail, Reason :" + e.Message);  
            }
            
        }
    }

}
