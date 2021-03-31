using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_32_ControlBirthNumberTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportErrorWhenInvalidBirthNumbersAreFound()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_32_ControlBirthNumber test = new A_32_ControlBirthNumber();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "19089328341")); // ok
            test.Run(new Field(fieldDefinition, "19089328342")); // invalid checksum
            test.Run(new Field(fieldDefinition, "08011129480")); // ok
            test.Run(new Field(fieldDefinition, "08011129481")); // invalid checkum
            test.Run(new Field(fieldDefinition, "08063048608")); // ok
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(2);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig fødselsnummer: 19089328342");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig fødselsnummer: 08011129481");
        }
    }
}