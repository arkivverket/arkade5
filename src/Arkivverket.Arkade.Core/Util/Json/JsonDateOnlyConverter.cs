using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arkivverket.Arkade.Core.Util.Json
{
    internal class JsonDateOnlyConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool isParsed = DateOnly.TryParse(reader.GetString(), out DateOnly parsedValue);

            return isParsed ? parsedValue : null;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy-MM-dd") ?? string.Empty);
        }
    }
}
