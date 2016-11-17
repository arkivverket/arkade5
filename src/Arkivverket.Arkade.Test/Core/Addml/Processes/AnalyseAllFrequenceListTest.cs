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
    public class AnalyseAllFrequenceListTest
    {
        [Fact]
        public void ShouldOnlyCreateFrequencyListOfCodeDefinitions()
        {
            AddmlFieldDefinition fieldDefinitionWithCodeList = new AddmlFieldDefinitionBuilder()
                .WithCodeList(new List<AddmlCode>
                {
                    new AddmlCode("A", ""),
                    new AddmlCode("B", ""),
                    new AddmlCode("C", ""),
                })
                .Build();
            AddmlFieldDefinition fieldDefinitionWithoutCodeList = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(fieldDefinitionWithCodeList.AddmlRecordDefinition)
                .Build();
            FlatFile flatFile = new FlatFile(fieldDefinitionWithCodeList.GetAddmlFlatFileDefinition());

            AnalyseAllFrequenceList test = new AnalyseAllFrequenceList();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinitionWithCodeList, "A"));
            test.Run(new Field(fieldDefinitionWithCodeList, "A"));
            test.Run(new Field(fieldDefinitionWithCodeList, "B"));
            test.Run(new Field(fieldDefinitionWithCodeList, "B"));
            test.Run(new Field(fieldDefinitionWithCodeList, "B"));
            test.Run(new Field(fieldDefinitionWithCodeList, "C"));
            test.Run(new Field(fieldDefinitionWithoutCodeList, "A"));
            test.Run(new Field(fieldDefinitionWithoutCodeList, "A"));
            test.Run(new Field(fieldDefinitionWithoutCodeList, "B"));
            test.Run(new Field(fieldDefinitionWithoutCodeList, "B"));
            test.Run(new Field(fieldDefinitionWithoutCodeList, "C"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinitionWithCodeList.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("2 forekomster av A");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinitionWithCodeList.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("3 forekomster av B");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinitionWithCodeList.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("1 forekomster av C");
        }
    }
}