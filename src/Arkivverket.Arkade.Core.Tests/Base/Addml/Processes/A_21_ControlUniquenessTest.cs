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
            test.Run(new Field(fieldDefinition, "A"), 1);
            test.Run(new Field(fieldDefinition, "B"), 1);
            test.Run(new Field(fieldDefinition, "C"), 1);
            test.Run(new Field(fieldDefinition, "D"), 1);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.TestResults.TestsResults[0].Message.Should().Be("Alle verdier er unike");
        }

        [Fact]
        public void ShouldReportNonUniquenessIfTwoFieldsHaveSameValue()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_21_ControlUniqueness test = new A_21_ControlUniqueness();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "A"), 1);
            test.Run(new Field(fieldDefinition, "B"), 1);
            test.Run(new Field(fieldDefinition, "C"), 1);
            test.Run(new Field(fieldDefinition, "A"), 2);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition.GetIndex()} - linje(r): 1, 2");
            testRun.TestResults.TestsResults[0].Message.Should().Be("A er ikke en unik verdi");
        }

    }
}