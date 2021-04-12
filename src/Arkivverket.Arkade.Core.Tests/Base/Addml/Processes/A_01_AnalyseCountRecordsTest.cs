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
    public class A_01_AnalyseCountRecordsTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportSuccessIfRecordCountIsCorrect()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithNumberOfRecords(4)
                .WithFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);

            A_01_AnalyseCountRecords test = new A_01_AnalyseCountRecords();
            test.Run(flatFile);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.Run((Record) null);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Message.Should().Be("Totalt: 4 poster");
        }
    }
}