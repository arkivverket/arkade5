﻿using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_17_ControlMinLengthTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportValuesShorterThanMinLength()
        {
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithMinLength(3)
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition.GetAddmlFlatFileDefinition());

            A_17_ControlMinLength test = new A_17_ControlMinLength();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition, "1"), 1);
            test.Run(new Field(fieldDefinition, "12"), 1);
            test.Run(new Field(fieldDefinition, "12"), 1);
            test.Run(new Field(fieldDefinition, "123"), 1);
            test.Run(new Field(fieldDefinition, "123"), 1);
            test.Run(new Field(fieldDefinition, "1234"), 1);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition.GetIndex()} - linje(r): 1");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Verdier kortere enn minstelengde: 1 12");
        }
    }
}