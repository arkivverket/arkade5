using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using static System.Environment;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_07_AnalyseCountNullTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindNullValues()
        {
            List<string> nullValues = new List<string> {"null", "<null>"};
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType(null, nullValues))
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_07_AnalyseCountNull test = new A_07_AnalyseCountNull();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "1"), 1);
            test.Run(new Field(fieldDefinition, "null"), 2);
            test.Run(new Field(fieldDefinition, "nulll"), 3);
            test.Run(new Field(fieldDefinition, "2"), 4);
            test.Run(new Field(fieldDefinition, "<null>"), 5);
            test.Run(new Field(fieldDefinition, ""), 6);
            test.Run(new Field(fieldDefinition, null), 7);
            test.Run(new Field(fieldDefinition, " "), 8);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.TestResults.TestsResults[0].Message.Should()
                .Be($"5 forekomster av null{NewLine}Gyldige nullverdier: 2{NewLine}Ugyldige nullverdier: 3");
        }
    }
}