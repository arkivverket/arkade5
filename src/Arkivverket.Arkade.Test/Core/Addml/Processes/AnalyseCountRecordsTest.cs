using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class AnalyseCountRecordsTest
    {
        [Fact]
        public void ShouldReportSuccessIfRecordCountIsCorrect()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithNumberOfRecords(4)
                .WithFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);

            AnalyseCountRecords test = new AnalyseCountRecords();
            test.Run(flatFile);
            test.Run((Arkade.Core.Addml.Record)null);
            test.Run((Arkade.Core.Addml.Record)null);
            test.Run((Arkade.Core.Addml.Record)null);
            test.Run((Arkade.Core.Addml.Record)null);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Message.Should().Be("RecordCount 4.");

        }


    }
}
