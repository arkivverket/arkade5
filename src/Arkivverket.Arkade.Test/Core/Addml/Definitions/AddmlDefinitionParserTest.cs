using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class AddmlDefinitionParserTest
    {

        [Fact]
        public void ShouldParseArkivuttrekkMedProsesser()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark3\\noark_3_arkivuttrekk_med_prosesser.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(3);
            addmlFlatFileDefinitions[0].Name.Should().Be("Saksregister");
            addmlFlatFileDefinitions[0].FileName.Should().Be("SAK.DAT");
            addmlFlatFileDefinitions[0].Charset.Should().Be("ISO-8859-1");
            addmlFlatFileDefinitions[0].RecordSeparator.Should().BeNull();
            addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
            addmlFlatFileDefinitions[0].Processes.Should().BeEmpty();

            AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
            addmlRecordDefinition.Processes.Should().BeEmpty();

            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(18);
            addmlFieldDefinitions[0].Name.Should().Be("Posttype");
            addmlFieldDefinitions[0].Processes.Should().Equal(
                "Analyse_FindMinMaxLengths",
                "Control_BirthNumber",
                "Control_Codes",
                "Analyse_FindMinMaxValues",
                "Analyse_FrequencyList"
            );

            addmlFieldDefinitions[1].Name.Should().Be("Grad");
            addmlFieldDefinitions[2].Name.Should().Be("Saksnr");
            addmlFieldDefinitions[3].Name.Should().Be("Dato");


            addmlRecordDefinition.PrimaryKey.Should().Equal(
                new List<AddmlFieldDefinition>() { addmlFieldDefinitions[2] }
            );


        }
    }
}
