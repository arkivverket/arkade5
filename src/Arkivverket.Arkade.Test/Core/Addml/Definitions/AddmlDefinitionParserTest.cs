using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class AddmlDefinitionParserTest
    {
        [Fact]
        public void ShouldParseJegerreg98ArkivuttrekkXml()
        {
            addml addml =
                ReadAddmlFile(
                    $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\jegerreg-98-dos\\arkivuttrekk.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(10);
            {
                addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
                addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
                addmlFlatFileDefinitions[0].Charset.Should().Be("ISO_8859_1");
                addmlFlatFileDefinitions[0].RecordSeparator.Should().Be("CRLF");
                addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(36);
                addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>() {addmlFieldDefinitions[0]});
            }
            {
                addmlFlatFileDefinitions[1].Name.Should().Be("ffd_4");
                addmlFlatFileDefinitions[1].FileName.Should().Be("ut_jeger.dat");
                addmlFlatFileDefinitions[1].Charset.Should().Be("ISO_8859_1");
                addmlFlatFileDefinitions[1].RecordSeparator.Should().Be("CRLF");
                addmlFlatFileDefinitions[1].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[1].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(37);
                addmlFieldDefinitions[0].Name.Should().Be("plassnummer");
                addmlFieldDefinitions[1].Name.Should().Be("landkort");
                addmlFieldDefinitions[8].Name.Should().Be("personnummer");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>()
                    {
                        addmlFieldDefinitions[0],
                        addmlFieldDefinitions[1],
                        addmlFieldDefinitions[8]
                    });
            }
            {
                addmlFlatFileDefinitions[2].Name.Should().Be("ffd_5");
                addmlFlatFileDefinitions[2].FileName.Should().Be("ikkejeg.dat");
                addmlFlatFileDefinitions[2].Charset.Should().Be("ISO_8859_1");
                addmlFlatFileDefinitions[2].RecordSeparator.Should().Be("CRLF");
                addmlFlatFileDefinitions[2].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[2].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(44);
                addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");
                addmlFieldDefinitions[0].ForeignKey.Should().Be(
                    addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0].AddmlFieldDefinitions[0]
                );

                addmlFieldDefinitions[1].Name.Should().Be("etternavn");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>()
                    {
                        addmlFieldDefinitions[0]
                    });
            }
        }

        [Fact]
        public void ShouldParseArkivuttrekkMedProsesser()
        {
            addml addml =
                ReadAddmlFile(
                    $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\addml\\noark_3_arkivuttrekk_med_prosesser.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(3);
            addmlFlatFileDefinitions[0].Name.Should().Be("Saksregister");
            addmlFlatFileDefinitions[0].FileName.Should().Be("SAK.DAT");
            addmlFlatFileDefinitions[0].Charset.Should().Be("ISO-8859-1");
            addmlFlatFileDefinitions[0].RecordSeparator.Should().BeNull();
            addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
            AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];

            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(18);
            addmlFieldDefinitions[0].Name.Should().Be("Posttype");
            addmlFieldDefinitions[1].Name.Should().Be("Grad");
            addmlFieldDefinitions[2].Name.Should().Be("Saksnr");
            addmlFieldDefinitions[3].Name.Should().Be("Dato");

            addmlRecordDefinition.PrimaryKey.Should().Equal(
                new List<AddmlFieldDefinition>() {addmlFieldDefinitions[2]});


            // TODO: Assert more asserts!
        }

        private static addml ReadAddmlFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            addml addml = SerializeUtil.DeserializeFromString<addml>(fileContent);
            return addml;
        }
    }
}