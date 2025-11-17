using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using diasMets = Arkivverket.Arkade.Core.ExternalModels.DiasMets.mets;
using submDescMets = Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription.mets;
using Arkivverket.Arkade.Core.Util;
using Newtonsoft.Json;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class MetadataLoader
    {
        public static ArchiveMetadata Load(string metadataFilePath)
        {
            using var streamReader = new StreamReader(metadataFilePath);
            string metadataFileContents = streamReader.ReadToEnd();

            switch (metadataFileContents[0]) // First character in file contents
            {
                case '{': // JSON
                case '[': // JSON
                    var metadata = JsonConvert.DeserializeObject<ArchiveMetadata>(metadataFileContents);
                    HandleLabelPlaceholder(metadata);
                    return metadata;
                case '<' // METS
                    when SerializeUtil.TryDeserializeFromString(metadataFileContents, out diasMets diasMets):
                    return DiasMetsLoader.Load(diasMets);
                // HandleLabelPlaceholder called from DiasMetsLoader
                case '<': // METS
                    var submissionDescription = SerializeUtil.DeserializeFromString<submDescMets>(metadataFileContents);
                    return SubmissionDescriptionLoader.Load(submissionDescription);
                // HandleLabelPlaceholder called from SubmissionDescriptionLoader
                default:
                    throw new ArkadeException($"The contents of {metadataFilePath} was not recognized as metadata");
            }
        }

        public static void HandleLabelPlaceholder(ArchiveMetadata metadata)
        {
            if (metadata.Label != null && metadata.Label.Equals(ArkadeConstants.MetadataStandardLabelPlaceholder))
                metadata.Label = ComposeStandardLabel(metadata.System.Name, metadata.StartDate, metadata.EndDate);
        }

        public static string ComposeStandardLabel(string systemName, DateTime? startDate, DateTime? endDate)
        {
            var standardLabel = new StringBuilder(systemName);

            if (!startDate.HasValue || !endDate.HasValue)
                return standardLabel.ToString();

            if (standardLabel.Length > 0) // SystemName is present
                standardLabel.Append(" ");

            standardLabel.Append($"({startDate.Value.Year} - {endDate.Value.Year})");

            return standardLabel.ToString();
        }
    }
}
