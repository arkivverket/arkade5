using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Newtonsoft.Json;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    internal class MetadataExampleGenerator
    {
        public void Generate(string outputFileName)
        {
            ArchiveMetadata metadataExample = CreateExampleData();

            string serializedObject = JsonConvert.SerializeObject(metadataExample,
                Formatting.Indented,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            WriteMetadataToFile(outputFileName, serializedObject);
        }

        private static ArchiveMetadata CreateExampleData()
        {
            var metadataExample = new ArchiveMetadata
            {
                Id = "UUID:12345-12345-12345-12345-12345-12345",
                ArchiveDescription = "test description",
                StartDate = new DateTime(2017, 5, 1),
                EndDate = new DateTime(2017, 8, 31),
                AgreementNumber = "Agreement number",
                System = new MetadataSystemInformationUnit
                {
                    Name = "Example System that supports Noark",
                    Version = "6.0",
                    Type = "Noark4"
                },
                Transferer = new MetadataEntityInformationUnit
                {
                    Entity = "Nav Telemark",
                    ContactPerson = "Hans 'The Transferer' Hansen",
                    Email = "hans@example.com",
                    Telephone = "12345678"
                },
                Producer = new MetadataEntityInformationUnit
                {
                    Entity = "Archive Solution Consulting Company",
                    ContactPerson = "Kari 'The Producer' Olsen",
                    Email = "kari@example.com",
                    Telephone = "12345678"
                },
                ArchiveCreators = new List<MetadataEntityInformationUnit>
                {
                    new MetadataEntityInformationUnit
                    {
                        Entity = "Arkivverket, seksjon Avlervering",
                        ContactPerson = "John 'The Creator' Johnsen",
                        Email = "john@example.com",
                        Telephone = "12345678"
                    }
                },
                Owners = new List<MetadataEntityInformationUnit>
                {
                    new MetadataEntityInformationUnit
                    {
                        Entity = "Nav Telemark",
                        ContactPerson = "Tim 'The Owner' Jensen",
                        Email = "tim@example.com",
                        Telephone = "12345678"
                    }
                },
                Comments = new List<string>()
                {
                    "Comments about the archive"
                },
                History = "A brief history"
            };
            return metadataExample;
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
                Log.Error(e, "Feil ved skriving av metadata til fil: " + e.Message);
            }
        }
    }
}