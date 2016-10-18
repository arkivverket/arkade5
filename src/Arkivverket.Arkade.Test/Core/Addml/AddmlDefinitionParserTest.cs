using System;
using Xunit;
using Arkivverket.Arkade.Core.Addml;
using System.IO;
using Arkivverket.Arkade.Util;
using Arkivverket.Arkade.ExternalModels.Addml;
using System.Collections.Generic;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class AddmlDefinitionParserTest
    {
        [Fact]
        public void ShouldParseJegerreg98ArkivuttrekkXml()
        {
            addml addml = ReadAddmlFile($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\jegerreg-98-dos\\arkivuttrekk.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(10);
            addmlFlatFileDefinitions[0].AddmlFieldDefinitions.Count.Should().Be(36);
            addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
            addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
            addmlFlatFileDefinitions[0].RecordLength.Should().Be(186);
            addmlFlatFileDefinitions[0].RecordSeparator.Should().Be("CRLF");
            addmlFlatFileDefinitions[0].FieldSeparator.Should().BeNull();
            addmlFlatFileDefinitions[0].Charset.Should().Be("ISO_8859_1");

            // TODO: Assert more addmlFlatFileDefinitions!
        }

        [Fact]
        public void ShouldParseArkivuttrekkMedProsesser()
        {
            addml addml = ReadAddmlFile($"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\addml\\noark_3_arkivuttrekk_med_prosesser.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(3);

            // TODO: Add asserts!
        }

        private static addml ReadAddmlFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            addml addml = SerializeUtil.DeserializeFromString<addml>(fileContent);
            return addml;
        }
    }
}
