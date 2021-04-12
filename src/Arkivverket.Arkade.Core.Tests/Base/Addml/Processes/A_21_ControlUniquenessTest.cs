using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_21_ControlUniquenessTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportUniquenessIfAllFieldsAreUniqe()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_21_ControlUniqueness test = new A_21_ControlUniqueness();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "C"));
            test.Run(new Field(fieldDefinition, "D"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Alle verdier er unike");
        }

        [Fact]
        public void ShouldReportNonUniquenessIfTwoFieldsHaveSameValue()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_21_ControlUniqueness test = new A_21_ControlUniqueness();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.Run(new Field(fieldDefinition, "C"));
            test.Run(new Field(fieldDefinition, "A"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Verdiene er ikke unike");
        }

    }
}