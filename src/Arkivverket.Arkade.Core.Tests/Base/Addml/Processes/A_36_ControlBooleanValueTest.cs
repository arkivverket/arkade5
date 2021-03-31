using System.Collections.Generic;
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
    public class A_36_ControlBooleanValueTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportNonBooleanValues()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Y/N", null))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(fieldDefinition1.AddmlRecordDefinition)
                .WithDataType(new BooleanDataType("Ja/Nei", new List<string>
                    {
                        "null"
                    }))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_36_ControlBooleanValue test = new A_36_ControlBooleanValue();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "Y"));
            test.Run(new Field(fieldDefinition1, "N"));
            test.Run(new Field(fieldDefinition1, "N"));
            test.Run(new Field(fieldDefinition1, "C"));
            test.Run(new Field(fieldDefinition1, "D"));
            test.Run(new Field(fieldDefinition2, "Y"));
            test.Run(new Field(fieldDefinition2, "J"));
            test.Run(new Field(fieldDefinition2, "Ja"));
            test.Run(new Field(fieldDefinition2, "null"));
            test.Run(new Field(fieldDefinition2, "Nei"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(2);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Følgende ikke-boolske verdier finnes: C D");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition2.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Følgende ikke-boolske verdier finnes: Y J");
        }
    }
}