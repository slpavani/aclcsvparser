using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CsvToFlatFileParser
{
    public class CsvParser
    {
        public static string outputFilePath = "";
        public static string logPath = "";
        private StreamReader reader;
        private StreamWriter outputFile;
        private StreamWriter logFile;
        private readonly List<List<string>> RowEntries = new List<List<string>>();
        private string currentEntry = "";
        private bool insideDoubleQuote;

        private const int TOTALCOLUMNS = 6;
        // Defining fixed length for each column.Assuming length is different for each column
        /* Note: We can also calculate length dynamically based on the maximum length of column value for each column in a file
           but it will be a heavy call.
        */

        #region "Column Lengths"
        private const int FirstNameLength = 15;
        private const int LastNameLength = 15;
        private const int SINLength = 11; 
        private const int DepartmentLength = 25;
        private const int TitleLength = 30;
        private const int SalaryLength = 9;
        private const int FixedLength = FirstNameLength + LastNameLength + SINLength + DepartmentLength + TitleLength + SalaryLength;
        #endregion "Column Lengths"

        /// <summary>
        ///   Reads CSV file in given path and writes all scanned entries to flat file(text).
        ///   File terminates if there is any exception and Logging is created
        /// </summary>
        public bool ScanFile(string filePath)
        {
            bool success = false;
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length <= 0)
                {
                    throw new Exception("Empty File.Terminating.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            GetOutputFilePath(filePath);

            try
            {
               success = ReadFile(filePath);
            }
            catch (Exception e)
            {
                logFile.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
            finally
            {
                outputFile.Close();
                logFile.Close();
            }

            return success;
        }

        /// <summary>
        /// checks if the column value is string or numeric type.Valid format : 12.1, 123, 1234.21
        /// </summary>
        /// <param name="isNumeric"></param>
        /// <returns></returns>
        public bool IsNumericType(string isNumeric)
        {
            Match m = Regex.Match(isNumeric, @"^\d+(\.\d{1,2})?$");
            return m.Success;
        }

        /// <summary>
        /// validate if SIN is in valid format or not.Vaild Format is xxx-xx-xxxx
        /// </summary>
        /// <param name="SIN"></param>
        /// <returns></returns>
        public bool IsValidSIN(string SIN)
        {
            if(SIN.Length!=SINLength)
                throw new Exception("Invalid SIN");

            Match m = Regex.Match(SIN, @"^\d{3}-\d{2}-\d{4}$");
            return m.Success;
        }

        #region "Private Methods"

        /// <summary>
        /// Reads ecah line from the input file and process it with defined validations
        /// </summary>
        /// <param name="filePath"></param>
        private bool ReadFile(string filePath)
        {
            bool success = false;
            using (var file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    // At the beginning of the line
                    if (!insideDoubleQuote)
                    {
                        RowEntries.Add(new List<string>());
                    }
                    // The characters of the line
                    foreach (char c in line)
                    {
                         if (insideDoubleQuote)
                         {
                            if (c == '"')
                            {
                                insideDoubleQuote = false;
                            }
                            else
                            {
                                currentEntry += c;
                            }
                        }
                        else if (c == ',')
                        {  
                            //Adding the string value to List
                            RowEntries[RowEntries.Count - 1].Add(currentEntry);
                            currentEntry = "";
                        }
                        else if (c == '"')
                        {
                            insideDoubleQuote = true;
                        }
                        else
                        {
                            currentEntry += c;
                        }
                    }

                    // At the end of the line
                    if (!insideDoubleQuote)
                    {
                        RowEntries[RowEntries.Count - 1].Add(currentEntry);
                        currentEntry = "";
                    }
                    else
                    {
                        currentEntry += "\n";
                    }

                    try
                    {
                        ProcessLine();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                  
                }
                return true;
            }

        }
        
        /// <summary>
        /// Precesses each line and checks if has valid no.of columns(6) or not
        /// </summary>
        private void  ProcessLine()
        {
            bool success;
            if (RowEntries.Count == 1)
            {
                List<string> newRow = RowEntries[RowEntries.Count - 1];

                if (newRow.Count == TOTALCOLUMNS)
                {
                     success = WriteLine(newRow);
                     RowEntries.Remove(newRow);
                }
                else if (newRow.Count > TOTALCOLUMNS)
                {
                    throw new Exception("Invalid File format. Number of columns (>6).Hence Terminating file parsing.." + string.Join(";", newRow));
                }

            }
            //code enters here in case of newline character in the column value and invalid no.of columns.
            else if ((RowEntries.Count < 1) || (RowEntries.Count > 1))
            {
                int entriesLength = RowEntries.Count;
                for (int i = entriesLength-1 ; i >= 0 ; i--)
                {
                    List<string> newRow = RowEntries[entriesLength - 1 - i];

                    if (newRow.Count == TOTALCOLUMNS)
                    {
                        success=WriteLine(newRow);
                        RowEntries.Remove(newRow);
                        entriesLength--;
                    }
                    //skips row if there are missing column values (<6) for a record not throwing any exception
                    else if (newRow.Count < TOTALCOLUMNS)
                    {
                        logFile.WriteLine("Invalid no.of columns (<6),hence skipping row - " + string.Join(";",newRow));
                        RowEntries.Remove(newRow);
                        entriesLength--;
                    }
                    //throws exception if no.of columns ( >6) for a record and terminates file reading
                    else
                    {
                        throw new Exception("Invalid File format. Number of columns (>6).Hence Terminating file parsing.." + string.Join(";", newRow));
                    }
                }

            }
        }

        /// <summary>
        /// Writes line to file if it meets below criteria
        /// 1.record length is equal to fixed length
        /// 2.the length of each field is valid
        /// 3.Column type is numeric field or not.Assuming TOTALCOLUMNS=6 then 1 to 5 columns are string and 6th column is numeric
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool WriteLine(List<string> row)
        {
            bool success = false;
            string column = "";
            int columnNumber = 1;
            int i;
            StringBuilder newRecord = new StringBuilder();
            for (i = 0; i <= row.Count - 1; i++)
            {
                column = row[i];
                bool numericType = IsNumericType(column);
               try
               { 
                   if (columnNumber % TOTALCOLUMNS != 0 && !numericType)
                   {
                        column = column.Replace("\n", " ");
                        //string field is saved as left aligned text
                        if (columnNumber % TOTALCOLUMNS == 1)
                        {
                            column = FormatColumn(column, FirstNameLength, numericType);
                        }
                        if (columnNumber % TOTALCOLUMNS == 2)
                        {
                            column = FormatColumn(column, LastNameLength, numericType);
                        }
                        if (columnNumber % TOTALCOLUMNS == 3)
                        {
                            bool validSIN = IsValidSIN(column);
                            if(validSIN)
                            column = FormatColumn(column, SINLength, numericType);
                            else
                            {
                                logFile.WriteLine("Invalid SIN format,hence skipping row " + string.Join(";", row));
                                return false;
                            }
                        }
                        if (columnNumber % TOTALCOLUMNS == 4)
                        {
                            column = FormatColumn(column, DepartmentLength, numericType);
                        }
                        if (columnNumber % TOTALCOLUMNS == 5)
                        {
                            column = FormatColumn(column, TitleLength, numericType);
                        }
                    }
                    else if (columnNumber % TOTALCOLUMNS == 0 && numericType)
                    {
                        //Numeric field is saved as right aligned text
                        column = FormatColumn(column, SalaryLength, numericType);
                    }
                    else
                    {
                        logFile.WriteLine("Invalid column type,hence skipping row " + string.Join(";", row));
                        return false;
                    }

                }
               catch (Exception e)
               {
                    logFile.WriteLine(e.Message + string.Join(";", row));
                    return false;
               }
                columnNumber++;
                newRecord.Append(column);
            }
            
            if (newRecord.ToString().Length == FixedLength)
            {
                    outputFile.WriteLine(newRecord.ToString());
                    success = true;
            }
            else
                    logFile.WriteLine("Row fixed length property is violated,hence skipping row " + string.Join(";", row));

            
            return success;
        }

        /// <summary>
        ///  Formats the column based on column type and column length.
        ///  Numeric field is saved as right aligned text and string field is saved as left aligned text.
        ///  if columnValue length is more than defined column length excepted throws error
        /// </summary>
        /// <param name="columnValue"></param>
        /// <param name="colLength"></param>
        /// <param name="numericType"></param>
        /// <returns></returns>
        private string FormatColumn(string columnValue, int colLength,bool numericType)
        {
            if (columnValue.Length < colLength)
            {
                if (numericType)
                {
                    columnValue = columnValue.PadLeft(colLength);
                }
                else
                {
                    columnValue = columnValue.PadRight(colLength);
                }
                
            }
                
            else if (columnValue.Length > colLength)
            {
                throw new Exception("Violation of column Length,hence skipping row - " );
            }

            return columnValue;
        }

        /// <summary>
        /// Creates new output file and log file in the same path as input file.
        /// Appends the data to the files if they already exists.
        /// </summary>
        /// <param name="filePath"></param>
        private void GetOutputFilePath(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            //If you don't want to append data to old file ,we can create new file everytime based on the timestamp.
            outputFilePath = path + "\\output.txt";
            logPath = path + "\\logFile.txt";

            try
            {
                logFile = !File.Exists(logPath) ? File.CreateText(logPath) : File.AppendText(logPath);
                if (!File.Exists(outputFilePath))
                {
                    outputFile = File.CreateText(outputFilePath);
                }
                else
                    outputFile = File.AppendText(outputFilePath);

                logFile.AutoFlush = true;
                outputFile.AutoFlush = true;
                logFile.WriteLine(DateTime.Now);
                //outputFile.WriteLine(DateTime.Now);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

       #endregion "Private Methods"
    }

}
