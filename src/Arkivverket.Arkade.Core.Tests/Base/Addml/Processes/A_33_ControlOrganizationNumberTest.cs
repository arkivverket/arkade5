using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_33_ControlOrganizationNumberTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportErrorWhenInvalidOrganizationNumbersAreFound()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_33_ControlOrganizationNumber test = new A_33_ControlOrganizationNumber();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "914994780")); // ok
            test.Run(new Field(fieldDefinition, "914994781")); // invalid checksum
            test.Run(new Field(fieldDefinition, "91499478")); // invalid length
            test.Run(new Field(fieldDefinition, "91499478A")); // invalid characters
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig organisasjonsnummer: 914994781");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig organisasjonsnummer: 91499478");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("Ugyldig organisasjonsnummer: 91499478A");
        }
    }
}