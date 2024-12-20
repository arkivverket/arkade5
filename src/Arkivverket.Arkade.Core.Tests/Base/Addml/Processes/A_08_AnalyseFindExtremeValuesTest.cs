﻿using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_08_AnalyseFindExtremeValuesTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldFindExtremeValues()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder().Build();
            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_08_AnalyseFindExtremeValues test = new A_08_AnalyseFindExtremeValues();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "1234567890"), 1);
            test.Run(new Field(fieldDefinition, "12345"), 1);
            test.Run(new Field(fieldDefinition, "1"), 1);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeTrue();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be(fieldDefinition.GetIndex().ToString());
            testRun.TestResults.TestsResults[0].Message.Should().Be("Lengste/korteste verdi: 10/1");
        }
    }
}