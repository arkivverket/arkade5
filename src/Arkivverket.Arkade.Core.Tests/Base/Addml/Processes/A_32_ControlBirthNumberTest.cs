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
            test.Run(new Field(fieldDefinition, "19089328341"), 1); // ok
            test.Run(new Field(fieldDefinition, "19089328342"), 1); // invalid checksum
            test.Run(new Field(fieldDefinition, "08011129480"), 1); // ok
            test.Run(new Field(fieldDefinition, "08011129481"), 1); // invalid checkum
            test.Run(new Field(fieldDefinition, "08063048608"), 1); // ok
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(2);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition.GetIndex()} - linje(r): 1");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig fødselsnummer: 19089328342");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition.GetIndex()} - linje(r): 1");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig fødselsnummer: 08011129481");
        }
    }
}