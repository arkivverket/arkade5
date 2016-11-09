using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlNumberOfRecordsTest
    {
        [Fact]
        public void ShouldReportSuccessIfNumberOfRecordsMatch()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithNumberOfRecords(4)
                .WithFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);

            ControlNumberOfRecords test = new ControlNumberOfRecords();
            test.Run(flatFile);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Message.Should().Be("Number of records (4) matched for file filnavn.dat.");
        }
    }
}