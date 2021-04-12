using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_35_ControlDateValueTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportNonDateValues()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_35_ControlDateValue test = new A_35_ControlDateValue();
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