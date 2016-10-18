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
            addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
            addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
            addmlFlatFileDefinitions[0].Charset.Should().Be("ISO_8859_1");
            addmlFlatFileDefinitions[0].RecordSeparator.Should().Be("CRLF");
            addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0].AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(36);
            addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");

            // TODO: Assert more asserts!
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
            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0].AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(18);
            addmlFieldDefinitions[0].Name.Should().Be("Posttype");

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