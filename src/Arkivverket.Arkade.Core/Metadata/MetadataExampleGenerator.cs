using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class MetadataExampleGenerator
    {
        public void Generate(string outputFileName)
        {
            ArchiveMetadata metadataExample = MetadataExampleCreator.Create(MetadataExamplePurpose.UserExample);

            string serializedObject = JsonConvert.SerializeObject(metadataExample,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters =
                    {
                        new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" },
                        new StringEnumConverter()
                    }
                });

            WriteMetadataToFile(outputFileName, serializedObject);
        }

        private static void WriteMetadataToFile(string outputFileName, string serializedObject)
        {
            try
            {
                var destination = new DirectoryInfo(outputFileName);
                File.WriteAllText(destination.FullName, serializedObject);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Could not write file {outputFileName}: {e.Message}");
            }
        }
    }
}
