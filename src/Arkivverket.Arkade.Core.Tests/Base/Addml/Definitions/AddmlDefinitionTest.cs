using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Internal;
using Arkivverket.Arkade.Core.ExternalModels.Cpf;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions
{
    public class AddmlDefinitionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AddmlDefinitionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldAddCollectPrimaryKeyToRecordDefinitionsContainingReferencedPrimaryKey()
        {
            AddmlFlatFileDefinition personFile = new AddmlFlatFileDefinitionBuilder().WithName("personFile").Build();
            AddmlRecordDefinition personFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(personFile).Build();
            AddmlFieldDefinition personPrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(personFileRecordDef)
                .WithName("personPrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlFlatFileDefinition addressFile = new AddmlFlatFileDefinitionBuilder().WithName("addressFile").Build();
            var addmlForeignKey = new AddmlForeignKey("testkey");
            
            AddmlRecordDefinition addressFileRecordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addressFile)
                .WithRecordProcess(ControlForeignKey.Name)
                .WithForeignKey(addmlForeignKey)
                .Build();

            new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef).WithName("addressPrimaryKey").IsPartOfPrimaryKey(true).Build();
            AddmlFieldDefinition addressForeignKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef)
                .WithName("addressForeignKey")
//                .WithForeignKey(personPrimaryKey)
                .Build();

            addmlForeignKey.ForeignKeys.Add(addressForeignKey);
            addmlForeignKey.ForeignKeyReferenceIndexes.Add(personPrimaryKey.GetIndex());
            addmlForeignKey.ForeignKeyReferenceFields.Add(personPrimaryKey);

            AddmlFlatFileDefinition unrelatedFile = new AddmlFlatFileDefinitionBuilder().WithName("unrelatedFile").Build();
            AddmlRecordDefinition unrelatedFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(unrelatedFile).Build();
            AddmlFieldDefinition unrelatedFilePrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(unrelatedFileRecordDef)
                .WithName("unrelatedFilePrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlDefinition addmlDefinition = new AddmlDefinition(new List<AddmlFlatFileDefinition>() {personFile, addressFile, unrelatedFile});

            Dictionary<IAddmlIndex, List<string>> recordProcesses = addmlDefinition.GetRecordProcessesGroupedByRecord();

            DebugPrintListOfProcesses(recordProcesses);

            recordProcesses[personFileRecordDef.GetIndex()].Contains(CollectPrimaryKey.Name).Should().BeTrue();
            recordProcesses[addressFileRecordDef.GetIndex()].Contains(ControlForeignKey.Name).Should().BeTrue();
            recordProcesses[unrelatedFileRecordDef.GetIndex()].Should().BeEmpty();
        }

        private void DebugPrintListOfProcesses(Dictionary<IAddmlIndex, List<string>> recordProcesses)
        {
            _testOutputHelper.WriteLine("record processes:");
            foreach (var item in recordProcesses)
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
        }

        [Fact]
        public void ShouldNotAddCollectPrimaryKeyProcessWhenControlForeignKeyIsNotDefinedInAnyRecordDefinition()
        {
            AddmlFlatFileDefinition personFile = new AddmlFlatFileDefinitionBuilder().WithName("personFile").Build();
            AddmlRecordDefinition personFileRecordDef = new AddmlRecordDefinitionBuilder().WithAddmlFlatFileDefinition(personFile).Build();
            AddmlFieldDefinition personPrimaryKey = new AddmlFieldDefinitionBuilder().WithRecordDefinition(personFileRecordDef)
                .WithName("personPrimaryKey")
                .IsPartOfPrimaryKey(true).Build();


            AddmlFlatFileDefinition addressFile = new AddmlFlatFileDefinitionBuilder().WithName("addressFile").Build();
            var addmlForeignKey = new AddmlForeignKey("testkey");
            AddmlRecordDefinition addressFileRecordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addressFile)
                .WithForeignKey(addmlForeignKey)
                .Build();

            new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef).WithName("addressPrimaryKey").IsPartOfPrimaryKey(true).Build();
            var addressField = new AddmlFieldDefinitionBuilder().WithRecordDefinition(addressFileRecordDef)
                .WithName("addressForeignKey")
                .Build();

            addmlForeignKey.ForeignKeys.Add(addressField);
            addmlForeignKey.ForeignKeyReferenceIndexes.Add(personPrimaryKey.GetIndex());
            addmlForeignKey.ForeignKeyReferenceFields.Add(personPrimaryKey);


            AddmlDefinition addmlDefinition = new AddmlDefinition(new List<AddmlFlatFileDefinition>() { personFile, addressFile });

            Dictionary<IAddmlIndex, List<string>> recordProcesses = addmlDefinition.GetRecordProcessesGroupedByRecord();

            DebugPrintListOfProcesses(recordProcesses);

            recordProcesses[personFileRecordDef.GetIndex()].Should().BeEmpty();
            recordProcesses[addressFileRecordDef.GetIndex()].Should().BeEmpty();
        }

    }
}
