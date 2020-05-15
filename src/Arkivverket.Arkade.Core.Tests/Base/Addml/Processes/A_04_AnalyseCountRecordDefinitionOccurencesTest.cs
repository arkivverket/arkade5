using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Base.Addml.Record;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_04_AnalyseCountRecordDefinitionOccurrencesTest
    {
        [Fact]
        public void ShouldCountRecordDefinitionOccurrences()
        {
            var process = new A_04_AnalyseCountRecordDefinitionOccurrences();
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