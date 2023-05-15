using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class MetadataExampleCreator
    {
        public static ArchiveMetadata Create(MetadataExamplePurpose purpose)
        {
            var metadataExample = new ArchiveMetadata // NB! Metadata-origin (metadata example creation)
            {
                Id = "UUID:12345-12345-12345-12345-12345-12345",
                Label = "Some system name (2017 - 2020)",
                ArchiveDescription = "Some archive description",
                AgreementNumber = "XX 00-0000/0000; 0000-00-00",
                RecordStatus = metsTypeMetsHdrRECORDSTATUS.NEW.ToString(),
                DeliveryType = "Sak-/Arkivsystem",
                ProjectName = "Some project name",
                PackageNumber = "1.0",
                ReferenceCode = "Some reference code",
                ArchiveCreators = new List<MetadataEntityInformationUnit>
                {
                    CreateMetadataEntityInformationUnit('1'),
                    CreateMetadataEntityInformationUnit('2')
                },
                Transferer = CreateMetadataEntityInformationUnit('3'),
                Producer = CreateMetadataEntityInformationUnit('4'),
                Owners = new List<MetadataEntityInformationUnit>
                {
                    CreateMetadataEntityInformationUnit('5'),
                    CreateMetadataEntityInformationUnit('6')
                },
                Creator = CreateMetadataEntityInformationUnit('7'),
                Recipient = "Some recipient",
                System = new MetadataSystemInformationUnit
                {
                    Name = "Some system name",
                    Version = "v1.0.0",
                    Type = "Noark5",
                    TypeVersion = "v3.1"
                },
                ArchiveSystem = new MetadataSystemInformationUnit
                {
                    Name = "Some archive system name",
                    Version = "v2.0.0",
                    Type = "Noark3",
                    TypeVersion = "N/A" // To be ignored by MetsCreator
                },
                CreatorSoftwareSystem = new MetadataSystemInformationUnit
                {
                    Name = "Some creator system name",
                    Version = "v3.0.0"
                },
                PackageType = PackageType.SubmissionInformationPackage,
                FileDescriptions = new List<FileDescription>
                {
                    new FileDescription
                    {
                        Id = 1,
                        Name = "someFileName.xml",
                        Extension = "xml",
                        Sha256Checksum = "3B29DFCC4286E50B180AF8F21904C86F8AA42A23C4055C3A71D0512F9AE3886F",
                        Size = 2325452,
                        ModifiedTime = new DateTime(2017, 06, 30)
                    },
                    new FileDescription
                    {
                        Id = 2,
                        Name = "content\\someFileName.xml",
                        Extension = "xml",
                        Sha256Checksum = "000CDCA105BD9722759FF81BCB2977E09E6A9A473735CCC540866989444198A2",
                        Size = 2427358,
                        ModifiedTime = new DateTime(2017, 06, 30)
                    }
                },
                StartDate = new DateTime(2017, 01, 01),
                EndDate = new DateTime(2020, 01, 01),
                ExtractionDate = new DateTime(2022, 01, 01),
            };

            if (purpose == MetadataExamplePurpose.UserExample)
                AdjustForUserExample(metadataExample);

            return metadataExample;
        }

        private static MetadataEntityInformationUnit CreateMetadataEntityInformationUnit(char distinctive)
        {
            return new MetadataEntityInformationUnit
            {
                Entity = $"Entity {distinctive}",
                ContactPerson = $"Contactperson {distinctive}",
                Address = $"Road {distinctive}, {distinctive}000 City",
                Telephone = $"{distinctive}-99999999",
                Email = $"post@entity-{Char.ToLower(distinctive)}.com"
            };
        }

        private static void AdjustForUserExample(ArchiveMetadata archiveMetadata)
        {
            archiveMetadata.Id = null;
            archiveMetadata.Label = ArkadeConstants.MetadataStandardLabelPlaceholder;
            archiveMetadata.FileDescriptions = null;
        }
    }

    public enum MetadataExamplePurpose
    {
        InternalTesting,
        UserExample
    }
}
