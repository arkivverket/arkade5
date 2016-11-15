using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class AnalyseCountCharsTest
    {
        [Fact]
        public void ShouldCountNumberOfCharsInFields()
        {
            AddmlFlatFileDefinition defintion = new AddmlFlatFileDefinitionBuilder()
                .WithFileName("filnavn.dat")
                .Build();
            FlatFile flatFile = new FlatFile(defintion);            

            AnalyseCountChars test = new AnalyseCountChars();
            test.Run(flatFile);
            test.Run(new Field(null, "1234567890"));
            test.Run(new Field(null, "12345"));
            test.Run(new Field(null, "1"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be("filnavn.dat");
            testRun.Results[0].Message.Should().Be("16 tegn");
        }
    }
}