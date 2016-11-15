using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class AnalyseCountRecordDefinitionOccurencesTest
    {
        [Fact]
        public void ShouldCountRecordDefinitionOccurences()
        {
            var process = new AnalyseCountRecordDefinitionOccurences();
            AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            FlatFile flatFile = new FlatFile(addmlFlatFileDefinition);

            var addmlRecordDefinition1 = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition)
                .WithName("recordDef1")
                .Build();
            var addmlRecordDefinition2 = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition)
                .WithName("recordDef2")
                .Build();

            Record record1 = new Record(addmlRecordDefinition1, new List<Field>());
            Record record2 = new Record(addmlRecordDefinition1, new List<Field>());
            Record record3 = new Record(addmlRecordDefinition2, new List<Field>());

            process.Run(flatFile);
            process.Run(record1);
            process.Run(record2);
            process.Run(record3);
            process.EndOfFile();
            TestRun testRun = process.GetTestRun();

            testRun.Results.Count.Should().Be(2);

            TestResult resultDef1 = testRun.Results.FirstOrDefault(r => r.Location.ToString().Contains("recordDef1"));
            TestResult resultDef2 = testRun.Results.FirstOrDefault(r => r.Location.ToString().Contains("recordDef2"));
            resultDef1.Should().NotBeNull();
            resultDef2.Should().NotBeNull();

            resultDef1?.Message.Should().Contain("2");
            resultDef2?.Message.Should().Contain("1");
        }
    }
}