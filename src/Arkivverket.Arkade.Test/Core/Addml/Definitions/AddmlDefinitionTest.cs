using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.ExternalModels.Cpf;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class AddmlDefinitionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AddmlDefinitionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldAddControlForeignKeyToFlatFileContainingReferencedPrimaryKey()
        {
            AddmlFlatFileDefinition personFile = new AddmlFlatFileDefinitionBuilder().WithName("personFile").Build();
            AddmlRecordDefinition personFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(personFile).Build();
            AddmlFieldDefinition personPrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(personFileRecordDef)
                .WithName("personPrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlFlatFileDefinition addressFile = new AddmlFlatFileDefinitionBuilder().WithName("addressFile").Build();
            AddmlRecordDefinition addressFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(addressFile).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef).WithName("addressPrimaryKey").IsPartOfPrimaryKey(true).Build();
            AddmlFieldDefinition addressForeignKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef)
                .WithName("addressForeignKey")
                .WithForeignKey(personPrimaryKey)
                .WithProcess(ControlForeignKey.Name)
                .Build();

            AddmlFlatFileDefinition unrelatedFile = new AddmlFlatFileDefinitionBuilder().WithName("unrelatedFile").Build();
            AddmlRecordDefinition unrelatedFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(unrelatedFile).Build();
            AddmlFieldDefinition unrelatedFilePrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(unrelatedFileRecordDef)
                .WithName("unrelatedFilePrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlDefinition addmlDefinition = new AddmlDefinition(new List<AddmlFlatFileDefinition>() {personFile, addressFile, unrelatedFile});

            Dictionary<IAddmlIndex, List<string>> fieldProcesses = addmlDefinition.GetFieldProcessesGroupedByField();

            _testOutputHelper.WriteLine("field processes:");
            foreach (var item in fieldProcesses)
            {
                _testOutputHelper.WriteLine(item.Key.ToString());
                if (item.Value == null || !item.Value.Any())
                {
                    _testOutputHelper.WriteLine("* [empty]");
                }
                else
                {
                    foreach (var process in item.Value)
                    {
                        _testOutputHelper.WriteLine($"* {process}");
                    }
                }
            }

            fieldProcesses[personPrimaryKey.GetIndex()].Contains(ControlForeignKey.Name).Should().BeTrue();
            fieldProcesses[addressForeignKey.GetIndex()].Contains(ControlForeignKey.Name).Should().BeTrue();
            fieldProcesses[unrelatedFilePrimaryKey.GetIndex()].Contains(ControlForeignKey.Name).Should().BeFalse();
        }

        [Fact]
        public void ShouldNotAddControlForeignKeyProcessWhenNotDefinedInAnyFieldDefinition()
        {
            AddmlFlatFileDefinition personFile = new AddmlFlatFileDefinitionBuilder().WithName("personFile").Build();
            AddmlRecordDefinition personFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(personFile).Build();
            AddmlFieldDefinition personPrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(personFileRecordDef)
                .WithName("personPrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlFlatFileDefinition addressFile = new AddmlFlatFileDefinitionBuilder().WithName("addressFile").Build();
            AddmlRecordDefinition addressFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(addressFile).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef).WithName("addressPrimaryKey").IsPartOfPrimaryKey(true).Build();
            AddmlFieldDefinition addressForeignKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef)
                .WithName("addressForeignKey")
                .WithForeignKey(personPrimaryKey)
                .Build();

            AddmlDefinition addmlDefinition = new AddmlDefinition(new List<AddmlFlatFileDefinition>() { personFile, addressFile });

            Dictionary<IAddmlIndex, List<string>> fieldProcesses = addmlDefinition.GetFieldProcessesGroupedByField();

            _testOutputHelper.WriteLine("field processes:");
            foreach (var item in fieldProcesses)
            {
                _testOutputHelper.WriteLine(item.Key.ToString());
                if (item.Value == null || !item.Value.Any())
                {
                    _testOutputHelper.WriteLine("* [empty]");
                }
                else
                {
                    foreach (var process in item.Value)
                    {
                        _testOutputHelper.WriteLine($"* {process}");
                    }
                }
            }

            fieldProcesses[personPrimaryKey.GetIndex()].Contains(ControlForeignKey.Name).Should().BeFalse();
            fieldProcesses[addressForeignKey.GetIndex()].Contains(ControlForeignKey.Name).Should().BeFalse();
        }

    }
}
