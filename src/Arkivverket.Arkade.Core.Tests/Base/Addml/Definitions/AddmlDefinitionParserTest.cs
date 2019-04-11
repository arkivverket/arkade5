using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Logging;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions
{
    public class AddmlDefinitionParserTest
    {
        public AddmlDefinitionParserTest()
        {
            var testDataDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\noark3");
            var workingDirectory = new WorkingDirectory(testDataDirectory, testDataDirectory);
            AddmlInfo addml = AddmlUtil.ReadFromFile(workingDirectory.Root().WithFile("noark_3_arkivuttrekk_med_prosesser.xml").FullName);
            _parser = new AddmlDefinitionParser(addml, workingDirectory, new StatusEventHandler());
        }

        private readonly AddmlDefinitionParser _parser;

        [Fact]
        public void ShouldParseAddmlWithForeignKey()
        {
            AddmlDefinition addmlDefinition = _parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(3);
            addmlFlatFileDefinitions[0].Name.Should().Be("Saksregister");

            AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
            addmlRecordDefinition.Name.Should().Be("Saksregisterpost");
            addmlRecordDefinition.Processes.Should().BeEmpty();

            addmlRecordDefinition.ForeignKeys.Should().NotBeNullOrEmpty();
            addmlRecordDefinition.ForeignKeys.Count.Should().Be(1);
            AddmlForeignKey foreignKey = addmlRecordDefinition.ForeignKeys[0];
            foreignKey.Name.Should().Be("FK1sak");
            foreignKey.ForeignKeys.Count.Should().Be(1);
            foreignKey.ForeignKeys[0].Name.Should().Be("Arkiv_2_delfelt");
            foreignKey.ForeignKeyReferenceIndexes[0].GetFlatFileDefinitionName().Should().Be("arkivnoekkelregister");
            foreignKey.ForeignKeyReferenceIndexes[0].GetRecordDefinitionName().Should().Be("arkivnoekkelregisterpost");
            foreignKey.ForeignKeyReferenceIndexes[0].GetFieldDefinitionName().Should().Be("arkivkode");

            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(18);
            addmlFieldDefinitions[5].Name.Should().Be("Arkiv_2_delfelt");
        }

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
        public void ShouldParseAddmlWithNumberOfRecordsProperty()
        {
            AddmlDefinition addmlDefinition = _parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;

            addmlFlatFileDefinitions[1].Name.Should().Be("Dokumentregister");
            addmlFlatFileDefinitions[1].NumberOfRecords.Should().Be(195);
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
            addmlRecordDefinition.Name.Should().Be("Saksregisterpost");
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
                new List<AddmlFieldDefinition> {addmlFieldDefinitions[2]}
            );
        }
    }
}