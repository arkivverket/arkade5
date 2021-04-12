using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_14_ControlNotUsedRecordDefTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportIfRecordDefinitionIsNotInUse()
        {
            AddmlFlatFileDefinition fileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            AddmlRecordDefinition recordDefinitionInUse = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(fileDefinition)
                .Build();
            AddmlRecordDefinition recordDefinitionNotInUse = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(fileDefinition)
                .Build();
            AddmlFieldDefinition fieldDefinitionInUse = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinitionInUse)
                .Build();
            AddmlFieldDefinition fieldDefinitionNotInUse = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinitionNotInUse)
                .Build();
            FlatFile flatFile = new FlatFile(fileDefinition);

            A_14_ControlNotUsedRecordDef test = new A_14_ControlNotUsedRecordDef();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinitionInUse, new List<Field> {
                new Field(fieldDefinitionInUse, "A"),
                new Field(fieldDefinitionInUse, "A"),
                new Field(fieldDefinitionInUse, "B")
            }));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(recordDefinitionNotInUse.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Posttypen er ikke i bruk");
        }

    }
}