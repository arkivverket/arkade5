﻿using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_13_ControlFixedLengthTest : LanguageDependentTest
    {
        [Fact]
        public void ShouldReportIfRecordLengthIsDifferentFromSpecified()
        {
            AddmlRecordDefinition recordDefiniton = new AddmlRecordDefinitionBuilder()
                .WithRecordLength(10)
                .Build();

            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefiniton)
                .Build();

            FlatFile flatFile = new FlatFile(recordDefiniton.AddmlFlatFileDefinition);

            A_13_ControlFixedLength test = new A_13_ControlFixedLength();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefiniton, 1, new List<Field> {
                new Field(fieldDefinition, "1"),
                new Field(fieldDefinition, "12"),
                new Field(fieldDefinition, "123"),
            }));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{recordDefiniton.GetIndex()} - linje(r): 1");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Oppgitt postlengde (10) er ulik faktisk (6)");
        }
    }
}