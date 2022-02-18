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
    public class A_20_ControlNotNullTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportNullValues()
        {
            List<string> nullValues = new List<string>
            {
                "null"
            };
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType(null, nullValues))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType(null, nullValues))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_20_ControlNotNull test = new A_20_ControlNotNull();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "A"), 1);
            test.Run(new Field(fieldDefinition1, "null"), 2);
            test.Run(new Field(fieldDefinition1, "B"), 3);
            test.Run(new Field(fieldDefinition1, "C"), 4);
            test.Run(new Field(fieldDefinition2, "A"), 1);
            test.Run(new Field(fieldDefinition2, "B"), 2);
            test.Run(new Field(fieldDefinition2, "C"), 3);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 2");
            testRun.TestResults.TestsResults[0].Message.Should().Be("NULL-verdier finnes");
        }
    }
}