using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_22_ControlCodesTest : LanguageDependentTest
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

            A_22_ControlCodes test = new A_22_ControlCodes();
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