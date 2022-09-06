using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Arkivverket.Arkade.Core.Util
{
    public static class CsvHelper
    {
        public static void WriteToFile<T, TMap>(string filePath, IEnumerable<T> records) where TMap : ClassMap<T>
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMap>();
            csv.WriteRecords(records);
        }

        public static void WriteToFile<T>(string filePath, IEnumerable<T> records)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(records);
        }

        public static string[] Split(string stringToSplit, string recordDelimiter, string fieldDelimiter, string quotingChar)
        {
            if (quotingChar == "\"")
            {
                using var csvReader = new CsvReader(new StringReader(stringToSplit),
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Quote = '"',
                        Delimiter = fieldDelimiter,
                        Escape = '"'
                    });
                csvReader.Read();
                return csvReader.Parser.Record;
            }
            string[] strings = stringToSplit.Split(fieldDelimiter, StringSplitOptions.None);

            return JoinQuoted(strings, recordDelimiter, fieldDelimiter, quotingChar);
        }

        private static string[] JoinQuoted(string[] strings, string recordDelimiter, string delimiter, string quotingChar)
        {
            if (string.IsNullOrEmpty(quotingChar))
                return strings;

            var fieldValues = new List<string>();
            var fieldValue = "";
            var concatenating = false;
            for (var i = 0; i < strings.Length; i++)
            {
                string splitFieldValue = strings[i];
                if (concatenating)
                {
                    fieldValue += delimiter;

                    if (EndsWithOddNumberOfQuotingChars(splitFieldValue, quotingChar))
                    {
                        fieldValue += splitFieldValue.TrimEnd(quotingChar);
                        fieldValues.Add(RemoveEscapeChars(fieldValue, quotingChar));
                        fieldValue = "";
                        concatenating = false;
                    }
                    else
                    {
                        fieldValue += splitFieldValue;
                    }
                }
                else
                {
                    // if not concatenating and last element, no concatenation needed - just add value
                    if (i == strings.Length - 1)
                    {
                        var valueWithoutRecordDelimiter = splitFieldValue.TrimEnd(recordDelimiter);
                        var valueWithoutQuotingChars = valueWithoutRecordDelimiter.Trim(quotingChar);
                        fieldValues.Add(RemoveEscapeChars(valueWithoutQuotingChars,
                            quotingChar));
                    }
                    else if (splitFieldValue.StartsWith(quotingChar))
                    {
                        // Remove leading quoting char in case split value only consists of quoting chars
                        if (EndsWithOddNumberOfQuotingChars(splitFieldValue.TrimStart(quotingChar), quotingChar))
                        {
                            fieldValues.Add(RemoveEscapeChars(splitFieldValue.Trim(quotingChar), quotingChar));
                        }
                        else
                        {
                            fieldValue = splitFieldValue.TrimStart(quotingChar);
                            concatenating = true;
                        }
                    }
                    else
                    {
                        fieldValues.Add(RemoveEscapeChars(splitFieldValue, quotingChar));
                    }
                }
            }

            return fieldValues.ToArray();
        }

        private static string RemoveEscapeChars(string value, string escape)
        {
            if (value == $"{escape}{escape}" || value == string.Empty)
            {
                return string.Empty;
            }
            for (int i; (i = value.IndexOf($"{escape}{escape}", StringComparison.InvariantCulture)) != -1;)
            {
                value = value.Remove(i, escape.Length);
            }

            return value;
        }

        private static bool EndsWithOddNumberOfQuotingChars(string value, string quotingChar)
        {
            var numberOfQuotingChars = 0;
            string copy = new(value);
            while (copy.EndsWith(quotingChar))
            {
                numberOfQuotingChars++;
                copy = copy.TrimEnd(quotingChar);
            }

            return numberOfQuotingChars % 2 == 1;
        }
    }
}
