using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Arkivverket.Arkade.Core.Report
{
    public class JsonReportGenerator : IReportGenerator
    {
        public void Generate(TestReport testReport, Stream stream)
        {
            var jsonWriterOptions = new JsonWriterOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
                Indented = true,
                SkipValidation = false,
            };

            using var jsonWriter = new Utf8JsonWriter(stream, jsonWriterOptions);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
            };

            JsonSerializer.Serialize(jsonWriter, testReport, jsonSerializerOptions);
        }
    }
}
