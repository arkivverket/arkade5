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
                    CreateMetadataEntityInformationUnit('A'),
                    CreateMetadataEntityInformationUnit('B')
                },
                Transferer = CreateMetadataEntityInformationUnit('E'),
                Producer = CreateMetadataEntityInformationUnit('F'),
                Owners =
                {
                    CreateMetadataEntityInformationUnit('C'),
                    CreateMetadataEntityInformationUnit('D')
                },
                Recipient = "Some recipient",
                System = new MetadataSystemInformationUnit
                {
                    Name = "Some system name",
                    Version = "vX.Y.Z",
                    Type = "Some system type",
                    TypeVersion = "vA.B.C"
                },
                ArchiveSystem = new MetadataSystemInformationUnit
                {
                    Name = "Some archive system name",
                    Version = "vX.Y.Z",
                    Type = "Some archive system type",
                    TypeVersion = "vA.B.C"
                },
                Comments = { "Some comment A", "Some comment B" },
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
                         agent.name.Equals("Entity A")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.CREATOR &&
                         agent.name.Equals("Contactperson A") &&
                         agent.note.Contains("A-99999999") &&
                         agent.note.Contains("post@entity-a.com")
            );

            // ARCHIVECREATOR 2: 

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST &&
                         agent.name.Equals("Entity B")
            );
            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.CREATOR &&
                         agent.name.Equals("Contactperson B") &&
                         agent.note.Contains("B-99999999") &&
                         agent.note.Contains("post@entity-b.com")
            );

            // TRANSFERER:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("SUBMITTER") &&
                         agent.name.Equals("Entity E")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("SUBMITTER") &&
                         agent.name.Equals("Contactperson E") &&
                         agent.note.Contains("E-99999999") &&
                         agent.note.Contains("post@entity-e.com")
            );

            // PRODUCER:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("PRODUCER") &&
                         agent.name.Equals("Entity F")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                         agent.OTHERROLE.Equals("PRODUCER") &&
                         agent.name.Equals("Contactperson F") &&
                         agent.note.Contains("F-99999999") &&
                         agent.note.Contains("post@entity-f.com")
            );

            // OWNER 1:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Entity C")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Contactperson C") &&
                         agent.note.Contains("C-99999999") &&
                         agent.note.Contains("post@entity-c.com")
            );

            // OWNER 2:

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Entity D")
            );

            metsHdrAgents.Should().Contain(
                agent => agent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL &&
                         agent.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                         agent.name.Equals("Contactperson D") &&
                         agent.note.Contains("D-99999999") &&
                         agent.note.Contains("post@entity-d.com")
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
                         && agent.name.Equals("vX.Y.Z")
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
                         && agent.name.Equals("vA.B.C")
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
                         agent.name.Equals("vX.Y.Z")
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
                         agent.name.Equals("vA.B.C")
            );

            // COMMENTS:

            /* Awaiting support in schema
            
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
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Metadata\\DiasMets";

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
