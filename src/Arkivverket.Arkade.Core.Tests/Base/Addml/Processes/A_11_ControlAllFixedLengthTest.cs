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
    public class A_11_ControlAllFixedLengthTest : LanguageDependentTest
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

            A_11_ControlAllFixedLength test = new A_11_ControlAllFixedLength();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefiniton, new List<Field> {
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