using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arkivverket.Arkade.Core.Util.Json
{
    internal class JsonDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool isParsed = DateTime.TryParse(reader.GetString(), out DateTime parsedValue);

            return isParsed ? parsedValue : null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy-MM-dd") ?? string.Empty);
        }
    }
}
