using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class AddmlDefinitionParserTest
    {
        public AddmlDefinitionParserTest()
        {
            AddmlInfo addml =
                AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark3\\noark_3_arkivuttrekk_med_prosesser.xml");
            _parser = new AddmlDefinitionParser(addml);
        }

        private readonly AddmlDefinitionParser _parser;

        [Fact]
        public void ShouldParseAddmlWithMultipleRecordDefinitions()
        {
            AddmlDefinition addmlDefinition = _parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;

            addmlFlatFileDefinitions[1].Name.Should().Be("Dokumentregister");
            addmlFlatFileDefinitions[1].AddmlRecordDefinitions.Count.Should().Be(2);
            addmlFlatFileDefinitions[1].AddmlRecordDefinitions[0].Name.Should().Be("Eksterne_dokumenter");
            addmlFlatFileDefinitions[1].AddmlRecordDefinitions[1].Name.Should().Be("Interne_dokumenter");
        }

        [Fact]
        public void ShouldParseAddmlWithProcesses()
        {
            AddmlDefinition addmlDefinition = _parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(3);
            addmlFlatFileDefinitions[0].Name.Should().Be("Saksregister");
            addmlFlatFileDefinitions[0].FileName.Should().Be("SAK.DAT");
            addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
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
                new List<AddmlFieldDefinition>() {addmlFieldDefinitions[2]}
            );
        }

        [Fact]
        public void ShouldParseAddmlWithNumberOfRecordsProperty()
        {
            AddmlDefinition addmlDefinition = _parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;

            addmlFlatFileDefinitions[1].Name.Should().Be("Dokumentregister");
            addmlFlatFileDefinitions[1].NumberOfRecords.Should().Be(195);
        }

    }
}