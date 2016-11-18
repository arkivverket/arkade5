using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlKeyTest
    {
        [Fact]
        public void ShouldReportIfKeyIsNotUnique()
        {
            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .Build();
            AddmlFieldDefinition fieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                .IsPartOfPrimaryKey(true)
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                .IsPartOfPrimaryKey(false)
                .Build();
            FlatFile flatFile = new FlatFile(recordDefinition.AddmlFlatFileDefinition);

            ControlKey test = new ControlKey();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "B"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "C"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "B"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "C"),
                new Field(fieldDefinition2, "C")
            }));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(recordDefinition.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Følgende primærnøkkelverdier er ikke unike: A,B A,C");
        }

    }
}