using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.DiasMets;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Test.Core;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Metadata
{
    public class DiasMetsCreatorTest
    {
        private readonly ArchiveMetadata _archiveMetadata;

        public DiasMetsCreatorTest()
        {
            _archiveMetadata = new ArchiveMetadata
            {
                ArchiveDescription = "Some archive description",
                AgreementNumber = "XX 00-0000/0000; 0000-00-00",
                ArchiveCreators =
                {
                    CreateMetadataEntityInformationUnit('1'),
                    CreateMetadataEntityInformationUnit('2')
                },
                Transferer = CreateMetadataEntityInformationUnit('3'),
                Producer = CreateMetadataEntityInformationUnit('4'),
                Owners =
                {
                    CreateMetadataEntityInformationUnit('5'),
                    CreateMetadataEntityInformationUnit('6')
                },
                Recipient = "Some recipient",
                System = new MetadataSystemInformationUnit
                {
                    Name = "Some system name",
                    Version = "v1.0.0",
                    Type = "Some system type",
                    TypeVersion = "v1.1.0"
                },
                ArchiveSystem = new MetadataSystemInformationUnit
                {
                    Name = "Some archive system name",
                    Version = "v2.0.0",
                    Type = "Some archive system type",
                    TypeVersion = "v2.1.0"
                },
                Comments = { "Some comment 1", "Some comment 2" },
            };
        }

        [Fact]
        public void ShouldCreateMetsFromMetadata()
        {
            mets mets = new DiasMetsCreator().Create(_archiveMetadata);

            metsTypeMetsHdr metsHdr = mets.metsHdr;

            // ARCHIVEDESCRIPTION:

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE.Equals("DELIVERYSPECIFICATION") &&
                altRecordId.Value.Equals("Some archive description")
            );

            // AGREEMENTNUMBER:

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE.Equals("SUBMISSIONAGREEMENT") &&
                altRecordId.Value.Equals("XX 00-0000/0000; 0000-00-00")
            );

            metsTypeMetsHdrAgent[] metsHdrAgents = metsHdr.agent;

            // ARCHIVECREATOR 1:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST &&
                         agent.name.Equals("Entity 1")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.CREATOR &&
                         agent.name.Equals("Contactperson 1") &&
                         agent.note.Contains("1-99999999") &&
                         agent.note.Contains("post@entity-1.com")
            );

            // ARCHIVECREATOR 2: 

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST &&
                         agent.name.Equals("Entity 2")
            );
            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.CREATOR &&
                         agent.name.Equals("Contactperson 2") &&
                         agent.note.Contains("2-99999999") &&
                         agent.note.Contains("post@entity-2.com")
            );

            // TRANSFERER:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("SUBMITTER") &&
                         agent.name.Equals("Entity 3")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("SUBMITTER") &&
                         agent.name.Equals("Contactperson 3") &&
                         agent.note.Contains("3-99999999") &&
                         agent.note.Contains("post@entity-3.com")
            );

            // PRODUCER:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("PRODUCER") &&
                         agent.name.Equals("Entity 4")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("PRODUCER") &&
                         agent.name.Equals("Contactperson 4") &&
                         agent.note.Contains("4-99999999") &&
                         agent.note.Contains("post@entity-4.com")
            );

            // OWNER 1:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Entity 5")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Contactperson 5") &&
                         agent.note.Contains("5-99999999") &&
                         agent.note.Contains("post@entity-5.com")
            );

            // OWNER 2:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Entity 6")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Contactperson 6") &&
                         agent.note.Contains("6-99999999") &&
                         agent.note.Contains("post@entity-6.com")
            );

            // RECIPIENT:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.PRESERVATION &&
                         agent.name.Equals("Some recipient")
            );

            // SYSTEM:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
                         && agent.name.Equals("Some system name")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
                         && agent.name.Equals("v1.0.0")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
                         && agent.name.Equals("Some system type")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
                         && agent.name.Equals("v1.1.0")
            );

            // ARCHIVE SYSTEM:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE == "PRODUCER"
                         && agent.name.Equals("Some archive system name")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE == "PRODUCER" &&
                         agent.name.Equals("v2.0.0")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE == "PRODUCER" &&
                         agent.name.Equals("Some archive system type")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                         agent.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE == "PRODUCER" &&
                         agent.name.Equals("v2.1.0")
            );

            // COMMENTS:

            /* TODO: Enable check for comments when they are supported in built in mets schema
            mets.amdSec.Any(a => a.techMD.Any(
                t1 => t1.mdWrap.Item.Equals("Some comment A")
                      && a.techMD.Any(
                          t2 => t2.mdWrap.Item.Equals("Some comment B")))
            ).Should().BeTrue();
            */
        }

        [Fact]
        public void ShouldSaveCreatedMetsfileToDisk()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Metadata\\DiasMetsCreator";

            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .Build();

            new DiasMetsCreator().CreateAndSaveFile(archive, _archiveMetadata);

            string metsFilePath = Path.Combine(workingDirectory, "dias-mets.xml");

            File.Exists(metsFilePath).Should().BeTrue();
        }

        private static MetadataEntityInformationUnit CreateMetadataEntityInformationUnit(char distinctive)
        {
            return new MetadataEntityInformationUnit
            {
                Entity = $"Entity {distinctive}",
                ContactPerson = $"Contactperson {distinctive}",
                Telephone = $"{distinctive}-99999999",
                Email = $"post@entity-{char.ToLower(distinctive)}.com"
            };
        }
    }
}
