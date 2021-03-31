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
    public class A_05_AnalyseAllFrequenceListTest : LanguageDependentTest
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

            A_05_AnalyseAllFrequenceList test = new A_05_AnalyseAllFrequenceList();
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