using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_02_AnalyseCountCharsTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldCountNumberOfCharsInFields()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithRelativeFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);            

            A_02_AnalyseCountChars test = new A_02_AnalyseCountChars();
            test.Run(flatFile);
            test.Run(new Field(null, "1234567890"),1);
            test.Run(new Field(null, "12345"),1);
            test.Run(new Field(null, "1"),1);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be("filnavn.dat");
            testRun.TestResults.TestsResults[0].Message.Should().Be("16 tegn");
        }
    }
}