using System.Collections.Generic;
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
    public class A_03_AnalyseFindExtremeRecordsTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindMaxAndMinLengthRecord()
        {
            AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithFieldSeparator("BB")
                .WithRecordSeparator("A")
                .Build();
            FlatFile flatFile = new FlatFile(flatFileDefinition);
            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(flatFileDefinition)
                .Build();
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                .Build();

            List<Field> fields1 = new List<Field>
            {
                new Field(fieldDefinition, "1234567890"),
                new Field(fieldDefinition, "12345"),
                new Field(fieldDefinition, "1")
            };
            Record record1 = new Record(recordDefinition, fields1);

            List<Field> fields2 = new List<Field>
            {
                new Field(fieldDefinition, "1"),
                new Field(fieldDefinition, ""),
                new Field(fieldDefinition, "3")
            };
            Record record2 = new Record(recordDefinition, fields2);


            A_03_AnalyseFindExtremeRecords test = new A_03_AnalyseFindExtremeRecords();
            test.Run(flatFile);
            test.Run(record1);
            test.Run(record2);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(recordDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Lengste/korteste post: 20/6");
        }
    }
}