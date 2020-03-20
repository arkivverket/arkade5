using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Mets;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class MetsCreatorTest
    {
        protected readonly ArchiveMetadata ArchiveMetadata;

        public MetsCreatorTest()
        {
            ArchiveMetadata = MetadataExampleCreator.Create(MetadataExamplePurpose.InternalTesting);
        }

        [Fact]
        public void ShouldCreateMetsFromMetadata()
        {
            mets mets = MetsCreator.Create(ArchiveMetadata);

            mets.LABEL.Should().Be("Some system name (2017 - 2020)");
            mets.OBJID.Should().Be("UUID:12345-12345-12345-12345-12345-12345");

            metsTypeMetsHdr metsHdr = mets.metsHdr;

            // CREATEDATE:

            metsHdr.CREATEDATE.Should().Be(new DateTime(2023, 01, 01));

            // ARCHIVEDESCRIPTION:

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE == metsTypeMetsHdrAltRecordIDTYPE.DELIVERYSPECIFICATION &&
                altRecordId.Value.Equals("Some archive description")
            );

            // AGREEMENTNUMBER:

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE == metsTypeMetsHdrAltRecordIDTYPE.SUBMISSIONAGREEMENT &&
                altRecordId.Value.Equals("XX 00-0000/0000; 0000-00-00")
            );

            // STARTDATE

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE == metsTypeMetsHdrAltRecordIDTYPE.STARTDATE &&
                altRecordId.Value.Equals("2017-01-01")
            );

            // ENDDATE

            metsHdr.altRecordID.Should().Contain(altRecordId =>
                altRecordId.TYPE == metsTypeMetsHdrAltRecordIDTYPE.ENDDATE &&
                altRecordId.Value.Equals("2020-01-01")
            );

            metsTypeMetsHdrAgent[] metsHdrAgents = metsHdr.agent;

            // ARCHIVECREATOR 1:

            metsHdrAgents[0].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[0].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[0].name.Should().Be("Entity 1");

            metsHdrAgents[1].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[1].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[1].name.Should().Be("Contactperson 1");
            metsHdrAgents[1].note.Should().Contain(n => n.Equals("Road 1, 1000 City"));
            metsHdrAgents[1].note.Should().Contain(n => n.Equals("1-99999999"));
            metsHdrAgents[1].note.Should().Contain(n => n.Equals("post@entity-1.com"));
            metsHdrAgents[1].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            // ARCHIVECREATOR 2: 

            metsHdrAgents[2].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[2].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[2].name.Should().Be("Entity 2");

            metsHdrAgents[3].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[3].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[3].name.Should().Be("Contactperson 2");
            metsHdrAgents[3].note.Should().Contain(n => n.Equals("Road 2, 2000 City"));
            metsHdrAgents[3].note.Should().Contain(n => n.Equals("2-99999999"));
            metsHdrAgents[3].note.Should().Contain(n => n.Equals("post@entity-2.com"));
            metsHdrAgents[3].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            // TRANSFERER:

            metsHdrAgents[4].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[4].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[4].OTHERROLE.Should().Be(metsTypeMetsHdrAgentOTHERROLE.SUBMITTER);
            metsHdrAgents[4].name.Should().Be("Entity 3");

            metsHdrAgents[5].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[5].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[5].OTHERROLE.Should().Be(metsTypeMetsHdrAgentOTHERROLE.SUBMITTER);
            metsHdrAgents[5].name.Should().Be("Contactperson 3");
            metsHdrAgents[5].note.Should().Contain(n => n.Equals("Road 3, 3000 City"));
            metsHdrAgents[5].note.Should().Contain(n => n.Equals("3-99999999"));
            metsHdrAgents[5].note.Should().Contain(n => n.Equals("post@entity-3.com"));
            metsHdrAgents[5].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            // PRODUCER:

            metsHdrAgents[6].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[6].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[6].OTHERROLE.Should().Be(metsTypeMetsHdrAgentOTHERROLE.PRODUCER);
            metsHdrAgents[6].name.Should().Be("Entity 4");

            metsHdrAgents[7].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[7].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[7].OTHERROLE.Should().Be(metsTypeMetsHdrAgentOTHERROLE.PRODUCER);
            metsHdrAgents[7].name.Should().Be("Contactperson 4");
            metsHdrAgents[7].note.Should().Contain(n => n.Equals("Road 4, 4000 City"));
            metsHdrAgents[7].note.Should().Contain(n => n.Equals("4-99999999"));
            metsHdrAgents[7].note.Should().Contain(n => n.Equals("post@entity-4.com"));
            metsHdrAgents[7].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            // OWNER 1:

            metsHdrAgents[8].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[8].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[8].name.Should().Be("Entity 5");

            metsHdrAgents[9].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[9].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[9].name.Should().Be("Contactperson 5");
            metsHdrAgents[9].note.Should().Contain(n => n.Equals("Road 5, 5000 City"));
            metsHdrAgents[9].note.Should().Contain(n => n.Equals("5-99999999"));
            metsHdrAgents[9].note.Should().Contain(n => n.Equals("post@entity-5.com"));

            // OWNER 2:

            metsHdrAgents[10].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[10].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[10].name.Should().Be("Entity 6");

            metsHdrAgents[11].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[11].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.IPOWNER);
            metsHdrAgents[11].name.Should().Be("Contactperson 6");
            metsHdrAgents[11].note.Should().Contain(n => n.Equals("Road 6, 6000 City"));
            metsHdrAgents[11].note.Should().Contain(n => n.Equals("6-99999999"));
            metsHdrAgents[11].note.Should().Contain(n => n.Equals("post@entity-6.com"));
            metsHdrAgents[11].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            // CREATOR:

            metsHdrAgents[12].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);//
            metsHdrAgents[12].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.CREATOR);
            metsHdrAgents[12].name.Should().Be("Entity 7");

            metsHdrAgents[13].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.INDIVIDUAL);
            metsHdrAgents[13].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.CREATOR);
            metsHdrAgents[13].name.Should().Be("Contactperson 7");
            metsHdrAgents[13].note.Should().Contain(n => n.Equals("Road 7, 7000 City"));
            metsHdrAgents[13].note.Should().Contain(n => n.Equals("7-99999999"));
            metsHdrAgents[13].note.Should().Contain(n => n.Equals("post@entity-7.com"));
            metsHdrAgents[13].note.Should().Contain(n => n.Equals("notescontent:Address,Telephone,Email"));

            //  CREATOR SOFTWARE SYSTEM

            metsHdrAgents[14].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[14].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.CREATOR);
            metsHdrAgents[14].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[14].name.Should().Be("Arkade 5");
            metsHdrAgents[14].note.Should().Contain(n => n.Equals($"{ArkadeVersion.Current}"));
            metsHdrAgents[14].note.Should().Contain(n => n.Equals("notescontent:Version"));

            // RECIPIENT:

            metsHdrAgents[15].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.ORGANIZATION);
            metsHdrAgents[15].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.PRESERVATION);
            metsHdrAgents[15].name.Should().Be("Some recipient");

            // SYSTEM:

            metsHdrAgents[16].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[16].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[16].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.ARCHIVIST);
            metsHdrAgents[16].name.Should().Be("Some system name");
            metsHdrAgents[16].note.Should().Contain(n => n.Equals("v1.0.0"));
            metsHdrAgents[16].note.Should().Contain(n => n.Equals("Noark5"));
            metsHdrAgents[16].note.Should().Contain(n => n.Equals("v3.1"));
            metsHdrAgents[16].note.Should().Contain(n => n.Equals("notescontent:Version,Type,TypeVersion"));

            // ARCHIVE SYSTEM:

            metsHdrAgents[17].TYPE.Should().Be(metsTypeMetsHdrAgentTYPE.OTHER);
            metsHdrAgents[17].OTHERTYPE.Should().Be(metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);
            metsHdrAgents[17].ROLE.Should().Be(metsTypeMetsHdrAgentROLE.OTHER);
            metsHdrAgents[17].OTHERROLE.Should().Be(metsTypeMetsHdrAgentOTHERROLE.PRODUCER);
            metsHdrAgents[17].name.Should().Be("Some archive system name");
            metsHdrAgents[17].note.Should().Contain(n => n.Equals("v2.0.0"));
            metsHdrAgents[17].note.Should().Contain(n => n.Equals("Noark4"));
            metsHdrAgents[17].note.Should().Contain(n => n.Equals("notescontent:Version,Type"));

            // Type-version applies to Noark5 only and is not expected amongst agents:
            metsHdrAgents.Length.Should().Be(18);

            // FILE DESCRIPTIONS:

            var metsFile = mets.fileSec.fileGrp[0].Items[0] as fileType;

            metsFile?.ID.Should().Be("fileId_1");
            metsFile?.MIMETYPE.Should().Be("application/pdf");
            metsFile?.USE.Should().Be("Datafile");
            metsFile?.CHECKSUMTYPE.Should().Be(fileTypeCHECKSUMTYPE.SHA256);
            metsFile?.CHECKSUM.Should().Be("3b29dfcc4286e50b180af8f21904c86f8aa42a23c4055c3a71d0512f9ae3886f");
            metsFile?.SIZE.Should().Be(2325452);
            metsFile?.CREATED.Year.Should().Be(2017);
            metsFile?.CREATED.Month.Should().Be(06);
            metsFile?.CREATED.Day.Should().Be(30);
            metsFile?.FLocat.href.Should().Be("file:someDirectory/someFileName.pdf");
            metsFile?.FLocat.LOCTYPE.Should().Be(mdSecTypeMdRefLOCTYPE.URL);

            // PACKAGE TYPE

            mets.TYPE.Should().Be(metsTypeTYPE.SIP);

            // MISCELLANEOUS:

            mets.structMap.Length.Should().Be(1);
        }
    }
}
