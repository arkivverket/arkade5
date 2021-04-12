using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Base.Addml.Record;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_12_ControlNumberOfRecordsTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportSuccessIfNumberOfRecordsMatch()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithNumberOfRecords(4)
                .WithFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);

            A_12_ControlNumberOfRecords test = new A_12_ControlNumberOfRecords();
            test.Run(flatFile);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Message.Should().Be("Number of records (4) matched");
        }
    }
}