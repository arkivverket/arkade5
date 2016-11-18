using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlDateValueTest
    {
        [Fact]
        public void ShouldReportNonDateValues()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDateValue test = new ControlDateValue();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "18.11.2016T08:43:00+01:00"));
            test.Run(new Field(fieldDefinition1, "notadate1"));
            test.Run(new Field(fieldDefinition1, "notadate2"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Verdier som ikke er dato: notadate1 notadate2");
        }
    }
}