using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class FlatFileReaderTest
    {

        [Fact]
        public void MultipleRecordDefinitionsPerflatFileShouldBeSupported()
        {
            FlatFile flatFile = GetDokDat();

            FixedFileFormatReader reader = new FixedFileFormatReader(flatFile);
            int numberOfRecords = 0;
            while (reader.MoveNext())
            {
                Record record = reader.Current;
                record.Should().NotBeNull();
                record.Fields[0].GetName().Should().Be("Posttype");
                record.Fields[0].Value.Should().BeOneOf("B", "N");

                string posttype = record.Fields[0].Value;
                if (posttype == "B")
                {
                    VerifyBrev(record);
                }
                else if (posttype == "N")
                {
                    VerifyNotat(record);
                }

                numberOfRecords++;
            }

            numberOfRecords.Should().Be(195);

            reader.MoveNext().Should().BeFalse();
        }

        private void VerifyBrev(Record record)
        {
            record.Fields.Count.Should().Be(24);

            record.Fields[0].GetName().Should().Be("Posttype");
            record.Fields[0].Value.Should().Be("B");
            record.Fields[1].GetName().Should().Be("Grad");
            record.Fields[1].Value.Should().Be("   ");
            record.Fields[2].GetName().Should().Be("Saksnr");
            record.Fields[2].Value.Should().MatchRegex("199./000..");
            record.Fields[3].GetName().Should().Be("Doknr");
            record.Fields[3].Value.Should().MatchRegex("0..");
            record.Fields[4].GetName().Should().Be("Journaldato");
            record.Fields[4].Value.Should().MatchRegex("199.....");
            record.Fields[5].GetName().Should().Be("Doktype");
            record.Fields[5].Value.Should().BeOneOf("I", "U");
            record.Fields[6].GetName().Should().Be("Uoff");
            record.Fields[6].Value.Should().Be("                ");
            record.Fields[7].GetName().Should().Be("Bdato");
            record.Fields[7].Value.Should().MatchRegex("199.....");
            record.Fields[8].GetName().Should().Be("Vedlegg");
            record.Fields[8].Value.Should().Be("  ");
            record.Fields[9].GetName().Should().Be("Avgradering");
            record.Fields[9].Value.Should().Be("   ");
            record.Fields[10].GetName().Should().Be("Saksbeh_for_dok_1_delfelt");
            record.Fields[10].Value.Should().Be("PA  ");
            record.Fields[11].GetName().Should().Be("Saksbeh_for_dok_2_delfelt");
            record.Fields[11].Value.Should().MatchRegex("... ");
            record.Fields[12].GetName().Should().Be("Blankt_felt_1");
            record.Fields[12].Value.Should().MatchRegex("       ");
            record.Fields[13].GetName().Should().Be("Avs_mot_forkortelse");
            record.Fields[13].Value.Should().BeOneOf("          ", "GRPO      ");
            record.Fields[14].GetName().Should().Be("Avs_mot");
            record.Fields[14].Value.Length.Should().Be(70);
            record.Fields[15].GetName().Should().Be("Dokumentbeskrivelse");
            record.Fields[15].Value.Length.Should().Be(140);
            record.Fields[16].GetName().Should().Be("Tillegg");
            record.Fields[16].Value.Length.Should().Be(210);
            record.Fields[17].GetName().Should().Be("Avskrivning_doknr");
            record.Fields[17].Value.Length.Should().Be(3);
            record.Fields[18].GetName().Should().Be("Avskrivning_dato");
            record.Fields[18].Value.Length.Should().Be(8);
            record.Fields[19].GetName().Should().Be("Avskrivning_maate");
            record.Fields[19].Value.Should().BeOneOf("    ", "BU  ", "TA  ");
            record.Fields[20].GetName().Should().Be("Loepenr");
            record.Fields[20].Value.Should().MatchRegex("0000../199.");
            record.Fields[21].GetName().Should().Be("Journalenhet");
            record.Fields[21].Value.Should().Be("GO ");
            record.Fields[22].GetName().Should().Be("Blankt_felt_2");
            record.Fields[22].Value.Should().Be("           ");
            record.Fields[23].GetName().Should().Be("Filreferanse");
            record.Fields[23].Value.Should().Be("                                        ");
        }

        private static void VerifyNotat(Record record)
        {
            record.Fields.Count.Should().Be(29);

            record.Fields[0].GetName().Should().Be("Posttype");
            record.Fields[0].Value.Should().Be("N");
            record.Fields[1].GetName().Should().Be("Grad");
            record.Fields[1].Value.Should().Be("   ");
            record.Fields[2].GetName().Should().Be("Saksnr");
            record.Fields[2].Value.Should().MatchRegex("199./000..");
            record.Fields[3].GetName().Should().Be("Doknr");
            record.Fields[3].Value.Should().MatchRegex("0..");
            record.Fields[4].GetName().Should().Be("Journaldato");
            record.Fields[4].Value.Should().MatchRegex("199.....");
            record.Fields[5].GetName().Should().Be("Doktype");
            record.Fields[5].Value.Should().Be("N");
            record.Fields[6].GetName().Should().Be("Uoff");
            record.Fields[6].Value.Should().Be("                ");
            record.Fields[7].GetName().Should().Be("Bdato");
            record.Fields[7].Value.Should().MatchRegex("199.....");
            record.Fields[8].GetName().Should().Be("Vedlegg");
            record.Fields[8].Value.Should().Be("  ");
            record.Fields[9].GetName().Should().Be("Avgradering");
            record.Fields[9].Value.Should().Be("   ");
            record.Fields[10].GetName().Should().Be("Saksbeh_for_dok_1_delfelt");
            record.Fields[10].Value.Should().Be("PA  ");
            record.Fields[11].GetName().Should().Be("Saksbeh_for_dok_2_delfelt");
            record.Fields[11].Value.Should().MatchRegex("... ");
            record.Fields[12].GetName().Should().Be("Blankt_felt_3");
            record.Fields[12].Value.Should().MatchRegex("       ");
            record.Fields[13].GetName().Should().Be("Internt_dok_fra");
            record.Fields[13].Value.Should()
                .BeOneOf("Bjørn Kragevik                ", "Stine Olsen                   ");
            record.Fields[14].GetName().Should().Be("Blankt_felt_4");
            record.Fields[14].Value.Should().Be("        ");
            record.Fields[15].GetName().Should().Be("Internt_dok_til");
            record.Fields[15].Value.Should().Be("GRPO                         ");
            record.Fields[16].GetName().Should().Be("Journalenhet_1");
            record.Fields[16].Value.Should().Be("   ");
            record.Fields[17].GetName().Should().Be("Behandler_1_delfelt");
            record.Fields[17].Value.Should().Be("GO  ");
            record.Fields[18].GetName().Should().Be("Behandler_2_delfelt");
            record.Fields[18].Value.Should().Be("ABC ");
            record.Fields[19].GetName().Should().Be("Blankt_felt_5");
            record.Fields[19].Value.Should().Be("  ");
            record.Fields[20].GetName().Should().Be("Dokumentbeskrivelse");
            record.Fields[20].Value.Length.Should().Be(140);
            record.Fields[21].GetName().Should().Be("Tillegg");
            record.Fields[21].Value.Length.Should().Be(210);
            record.Fields[22].GetName().Should().Be("Blankt_felt_6");
            record.Fields[22].Value.Should().Be("   ");
            record.Fields[23].GetName().Should().Be("Avskrivning_dato");
            record.Fields[23].Value.Should().MatchRegex("        ");
            record.Fields[24].GetName().Should().Be("Avskrivning_maate");
            record.Fields[24].Value.Should().BeOneOf("    ", "BU  ", "TA  ");
            record.Fields[25].GetName().Should().Be("Loepenr");
            record.Fields[25].Value.Should().MatchRegex("0000../199.");
            record.Fields[26].GetName().Should().Be("Journalenhet_2");
            record.Fields[26].Value.Should().Be("GO ");
            record.Fields[27].GetName().Should().Be("Blankt_felt_7");
            record.Fields[27].Value.Should().Be("           ");
            record.Fields[28].GetName().Should().Be("Filreferanse");
            record.Fields[28].Value.Should().Be("                                        ");
        }

        private FlatFile GetDokDat()
        {

            var workingDirectory = new WorkingDirectory(ArkadeConstants.GetArkadeWorkDirectory(), new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\TestData\\noark3\\"));
            AddmlInfo addml = AddmlUtil.ReadFromFile(workingDirectory.Content().WithFile("noark_3_arkivuttrekk_med_prosesser.xml").FullName);

            AddmlDefinition addmlDefinition = new AddmlDefinitionParser(addml, workingDirectory, new StatusEventHandler()).GetAddmlDefinition();
            FlatFile flatFile = addmlDefinition.GetFlatFiles().Single(file => file.Definition.FileName == "DOK.DAT");
            return flatFile;
        }

    }
}