using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlAccountNumberTest
    {
        [Fact]
        public void ShouldReportErrorWhenInvalidBirthNumbersAreFound()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlAccountNumber test = new ControlAccountNumber();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "12345678903")); // ok
            test.Run(new Field(fieldDefinition, "12345678901")); // invalid checksum
            test.Run(new Field(fieldDefinition, "1234567890")); // invalid length
            test.Run(new Field(fieldDefinition, "1234567890A")); // invalid characters
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig kontonummer: 12345678901");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig kontonummer: 1234567890");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("Ugyldig kontonummer: 1234567890A");
        }
    }
}