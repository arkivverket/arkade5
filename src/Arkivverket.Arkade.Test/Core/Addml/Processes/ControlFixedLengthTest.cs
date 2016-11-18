using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlFixedLengthTest
    {
        [Fact]
        public void ShouldReportIfRecordLengthIsDifferentFromSpecified()
        {
            AddmlRecordDefinition recordDefiniton = new AddmlRecordDefinitionBuilder()
                .WithRecordLength(10)
                .Build();

            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefiniton)
                .Build();

            FlatFile flatFile = new FlatFile(recordDefiniton.AddmlFlatFileDefinition);

            ControlFixedLength test = new ControlFixedLength();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Addml.Record(recordDefiniton, new List<Field> {
                new Field(fieldDefinition, "1"),
                new Field(fieldDefinition, "12"),
                new Field(fieldDefinition, "123"),
            }));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(recordDefiniton.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Oppgitt postlengde (10) er ulik faktisk (6)");
        }
    }
}