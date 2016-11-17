using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlCodesTest
    {
        [Fact]
        public void ShouldVerifyThatOnlyCodesAreUsed()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithCodeList(new List<AddmlCode>
                {
                    new AddmlCode("Y", ""),
                    new AddmlCode("N", "")
                })
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            ControlCodes test = new ControlCodes();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "Y"));
            test.Run(new Field(fieldDefinition, "N"));
            test.Run(new Field(fieldDefinition, "A"));
            test.Run(new Field(fieldDefinition, "B"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ikke i kodelisten: A B");
        }
    }
}