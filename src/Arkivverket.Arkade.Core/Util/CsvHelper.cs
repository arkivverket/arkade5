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
    }
}
