using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Integration
{
    public class Noark4Test
    {
        [Fact]
        public void ShouldParseNoark4ConvertedArkivuttrekkXml()
        {
            // Code to convert NOARKIH.XML to addml.xml
            //string noarkihString = TestUtil.ReadFromFileInTestDataDir("noark4\\NOARKIH.XML");
            //string addmlString = NoarkihToAddmlTransformer.Transform(noarkihString);

            // File is converted from NOARKIH.XML format
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark4\\addml.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();

            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(63);
            addmlFlatFileDefinitions[0].Name.Should().Be("ORDNPRINSTYPE");
            addmlFlatFileDefinitions[0].FileName.Should().Be("OPRITYP.XML");
            addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
            addmlFlatFileDefinitions[0].RecordSeparator.Should().BeNull();
            addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
            AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
            addmlRecordDefinition.Name.Should().Be("ORDNPRINSTYPE");
            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(2);
            addmlFieldDefinitions[0].Name.Should().Be("OT.KODE");
            addmlFieldDefinitions[0].Type.Should().Be(StringDataType.Default);
            addmlFieldDefinitions[1].Name.Should().Be("OT.BETEGN");
            addmlFieldDefinitions[1].Type.Should().Be(StringDataType.Default);
            addmlRecordDefinition.PrimaryKey.Should().BeNull();
        }
    }
}