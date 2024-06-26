﻿using System.Collections.Generic;
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
    public class A_36_ControlBooleanValueTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportNonBooleanValues()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Y/N", null))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(fieldDefinition1.AddmlRecordDefinition)
                .WithDataType(new BooleanDataType("Ja/Nei", new List<string>
                    {
                        "null"
                    }))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_36_ControlBooleanValue test = new A_36_ControlBooleanValue();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "Y"), 1);
            test.Run(new Field(fieldDefinition2, "Y"), 1);
            test.Run(new Field(fieldDefinition1, "N"), 2);
            test.Run(new Field(fieldDefinition2, "J"), 2);
            test.Run(new Field(fieldDefinition1, "N"), 3);
            test.Run(new Field(fieldDefinition2, "Ja"), 3);
            test.Run(new Field(fieldDefinition1, "C"), 4);
            test.Run(new Field(fieldDefinition2, "null"), 4);
            test.Run(new Field(fieldDefinition1, "D"), 5);
            test.Run(new Field(fieldDefinition2, "Nei"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(4);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 1");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Følgende ikke-boolske verdier finnes: Y");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 2");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Følgende ikke-boolske verdier finnes: J");
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 4");
            testRun.TestResults.TestsResults[2].Message.Should().Be("Følgende ikke-boolske verdier finnes: C");
            testRun.TestResults.TestsResults[3].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[3].Message.Should().Be("Følgende ikke-boolske verdier finnes: D");
        }
    }
}