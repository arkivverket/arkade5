using System;
using System.Collections.Generic;
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
                    Type = "Noark4",
                    TypeVersion = "N/A" // To be ignored by DiasMetsCreator
                },
                Comments = new List<string> { "Some comment 1", "Some comment 2" }
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

            metsHdrAgents[0].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[0].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[0].name.Should().Be("Entity 1");

            metsHdrAgents[1].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[1].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[1].name.Should().Be("Contactperson 1");

            metsHdrAgents[2].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[2].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[2].note.First().Should().Be("1-99999999");

            metsHdrAgents[3].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[3].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[3].note.First().Should().Be("post@entity-1.com");

            // ARCHIVECREATOR 2: 

            metsHdrAgents[4].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[4].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[4].name.Should().Be("Entity 2");

            metsHdrAgents[5].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[5].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[5].name.Should().Be("Contactperson 2");

            metsHdrAgents[6].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[6].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[6].note.First().Should().Be("2-99999999");

            metsHdrAgents[7].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[7].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[7].note.First().Should().Be("post@entity-2.com");

            // TRANSFERER:

            metsHdrAgents[8].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[8].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[8].OTHERROLE.Should().Be("SUBMITTER");
            metsHdrAgents[8].name.Should().Be("Entity 3");

            metsHdrAgents[9].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[9].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[9].OTHERROLE.Should().Be("SUBMITTER");
            metsHdrAgents[9].name.Should().Be("Contactperson 3");

            metsHdrAgents[10].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[10].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[10].OTHERROLE.Should().Be("SUBMITTER");
            metsHdrAgents[10].note.First().Should().Be("3-99999999");

            metsHdrAgents[11].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[11].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[11].OTHERROLE.Should().Be("SUBMITTER");
            metsHdrAgents[11].note.First().Should().Be("post@entity-3.com");

            // PRODUCER:

            metsHdrAgents[12].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[12].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[12].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[12].name.Should().Be("Entity 4");

            metsHdrAgents[13].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[13].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[13].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[13].name.Should().Be("Contactperson 4");

            metsHdrAgents[14].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[14].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[14].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[14].note.First().Should().Be("4-99999999");

            metsHdrAgents[15].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[15].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[15].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[15].note.First().Should().Be("post@entity-4.com");

            // OWNER 1:

            metsHdrAgents[16].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[16].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[16].name.Should().Be("Entity 5");

            metsHdrAgents[17].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[17].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[17].name.Should().Be("Contactperson 5");

            metsHdrAgents[18].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[18].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[18].note.First().Should().Be("5-99999999");

            metsHdrAgents[19].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[19].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[19].note.First().Should().Be("post@entity-5.com");

            // OWNER 2:

            metsHdrAgents[20].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[20].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[20].name.Should().Be("Entity 6");

            metsHdrAgents[21].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[21].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[21].name.Should().Be("Contactperson 6");

            metsHdrAgents[22].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[22].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[22].note.First().Should().Be("6-99999999");

            metsHdrAgents[23].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[23].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[23].note.First().Should().Be("post@entity-6.com");

            // RECIPIENT:

            metsHdrAgents[24].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[24].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.PRESERVATION);
            metsHdrAgents[24].name.Should().Be("Some recipient");

            // SYSTEM:

            metsHdrAgents[25].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[25].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[25].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[25].name.Should().Be("Some system name");

            metsHdrAgents[26].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[26].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[26].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[26].note.First().Should().Be("v1.0.0");

            metsHdrAgents[27].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[27].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[27].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[27].note.First().Should().Be("Noark5");

            metsHdrAgents[28].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[28].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[28].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[28].note.First().Should().Be("v3.1");

            // ARCHIVE SYSTEM:

            metsHdrAgents[29].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[29].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[29].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[29].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[29].name.Should().Be("Some archive system name");

            metsHdrAgents[30].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[30].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[30].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[30].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[30].note.First().Should().Be("v2.0.0");

            metsHdrAgents[31].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[31].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[31].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[31].OTHERROLE.Should().Be("PRODUCER");
            metsHdrAgents[31].note.First().Should().Be("Noark4");

            // Type-version applies to Noark5 only and is not expected amongst agents:
            metsHdrAgents.Length.Should().Be(32);

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
