using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlNotNullTest
    {
        [Fact]
        public void ShouldReportNullValues()
        {
            List<string> nullValues = new List<string>
            {
                "null"
            };
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("", nullValues))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("", nullValues))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlNotNull test = new ControlNotNull();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "A"));
            test.Run(new Field(fieldDefinition1, "null"));
            test.Run(new Field(fieldDefinition1, "B"));
            test.Run(new Field(fieldDefinition1, "C"));
            test.Run(new Field(fieldDefinition2, "A"));
            test.Run(new Field(fieldDefinition2, "B"));
            test.Run(new Field(fieldDefinition2, "C"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("NULL-verdier finnes");
        }
    }
}