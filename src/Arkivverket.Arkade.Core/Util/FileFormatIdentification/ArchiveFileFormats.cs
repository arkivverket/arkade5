using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly:InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    internal class ArchiveFileFormats
    {
        [JsonPropertyName("regulation_version")]
        public string RegulationVersion { get; set; }
        [JsonPropertyName("file_formats")]
        public IEnumerable<ArchiveFileFormat> FileFormats { get; set; }
    }

    internal class ArchiveFileFormat
    {
        [JsonPropertyName("puid")]
        public IEnumerable<string> Puid { get; set; }
        [JsonPropertyName("valid_to")]
        public DateOnly? ValidTo { get; set; }
        [JsonPropertyName("valid_from")]
        public DateOnly? ValidFrom { get; set; }
        [JsonPropertyName("additional_requirements")]
        public string AdditionalRequirements { get; set; }
    }
}