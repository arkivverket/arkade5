using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Arkivverket.Arkade.Core.Report
{
    public class JsonReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, StreamWriter stream)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
                WriteIndented = true,
                IgnoreReadOnlyFields = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
            };

            string jsonReport = JsonSerializer.Serialize(testReport, jsonSerializerOptions);

            stream.Write(jsonReport);
            stream.Flush();
        }
    }
}
