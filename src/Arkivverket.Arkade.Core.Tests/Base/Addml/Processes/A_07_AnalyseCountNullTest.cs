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
            test.Run(new Field(fieldDefinition, "1"));
            test.Run(new Field(fieldDefinition, "null"));
            test.Run(new Field(fieldDefinition, "nulll"));
            test.Run(new Field(fieldDefinition, "2"));
            test.Run(new Field(fieldDefinition, "<null>"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("2 forekomster av null");
        }
    }
}