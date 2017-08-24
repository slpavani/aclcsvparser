using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvToFlatFileParser
{
    class Class2
    {
        public static string outputFilePath = "";
        public static string logPath = "";
        private StreamReader reader;
        private StreamWriter outputFile;
        private StreamWriter logFile;
        private const int FirstNameLength = 15;
        private const int LastNameLength = 15;
        private const int SINLength = 12;
        private const int DepartmentLength = 25;
        private const int TitleLength = 30;
        private const int SalaryLength = 9;
        private const int FixedLength = FirstNameLength + LastNameLength + SINLength + DepartmentLength + TitleLength + SalaryLength;
        /// <summary>

        public void scanFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            FileInfo fi = new FileInfo(filePath);
            if (fi.Length <= 0)
            {
                throw new Exception("empty file");
            }
            getOutputFilePath(filePath);
            using (reader = new StreamReader(filePath))
            {
                char ch;
                List<string> listOfWords = new List<string>();
                int wordCount = 1;
                do
                {
                    ch = Read();
                    if (ch != '"')
                    {
                        // invalid wword;

                    }
                    string word = "";
                    do
                    {
                        ch = Read();
                        if (ch != '"')
                        {
                            word += ch;
                        }

                    } while (ch != '"');
                    listOfWords.Add(word);

                    if (wordCount % 6 == 0)
                    {
                        ch = Read();
                        if (ch != '\r')
                        {
                            // invalid input
                        }
                        ch = Read();
                        if (ch != '\n')
                        {
                            // invalid input
                        }
                        processLine(listOfWords);
                        listOfWords.Clear();
                    }
                    else
                    {
                        ch = Read();
                        if (ch != ',')
                        {
                            // invalid input
                        }
                    }
                    wordCount++;
                } while (!reader.EndOfStream);


                // while (!reader.EndOfStream);




                // WriteToFile(, );
                //else
                //    throw new Exception("No data to write to file");

            }

        }

        private char Read()
        {
            char ch;
            if (reader.EndOfStream)
            {
                // throw exception
            }
            ch = (char) reader.Read();
            return ch;

        }

        private void processLine(List<string> row)
        {
            int i;
            int rownumber = 0;
            string column = "";
            if (row.Count == 6)
            {
                StringBuilder newRecord = new StringBuilder();
                for (i = 0; i < row.Count; i++)
                {
                    column = row[i];
                    column = column.Replace("\r\n", " ");
                    bool numericType = IsNumericType(column);
                    if (i != 5 && !numericType)
                    {
                        
                        //row[i] = row[i].Replace("", "");
                        //string field is saved as left aligned text
                        if (i == 0)
                        {
                          //  valdiate(column, FirstNameLength);
                        }
                    }
                
                    else if (i == 5  && numericType)
                    {
                        //Numeric field is saved as right aligned text
                        
                    }
                    else
                    {
                        logFile.WriteLine("invalid column type,hence skipping the row :" + rownumber.ToString());
                        continue;
                    }
                    newRecord.Append(column);
                    //newRecord.Append(" ");
                }
                //if (newRecord.ToString().Length == FixedLength)
                    outputFile.WriteLine(newRecord.ToString());
                //else
                //    logFile.WriteLine("Row fixed length property is violated,hence skipping the row" + rownumber);
            }
            else
            {
                logFile.WriteLine("invalid number of columns for the record,hence skipping the row" + rownumber);
                return;
            }


        }

        private void getOutputFilePath(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            outputFilePath = path + "\\Copy.txt";
            logPath = path + "\\logFile.txt";
            

            logFile = !File.Exists(logPath) ? File.CreateText(logPath) : File.AppendText(logPath);
            if (!File.Exists(outputFilePath))
            {
                outputFile = File.CreateText(outputFilePath);
            }
            else
                outputFile=File.AppendText(outputFilePath);

            logFile.AutoFlush = true;
            outputFile.AutoFlush = true;
        }
        private static bool IsNumericType(string o)
        {
            Match m = Regex.Match(o, @"^\d+(\.\d{1,2})?$");
            return m.Success;
        }
        //private static string Validate(string columnvalue,int)
    }
}
