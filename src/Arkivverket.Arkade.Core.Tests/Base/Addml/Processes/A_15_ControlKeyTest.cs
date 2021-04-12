using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_15_ControlKeyTest : LanguageDependentTest
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

            A_15_ControlKey test = new A_15_ControlKey();
            test.Run(flatFile);
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "B"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "C"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinition, new List<Field> {
                new Field(fieldDefinition, "A"),
                new Field(fieldDefinition, "B"),
                new Field(fieldDefinition2, "B")
            }));
            test.Run(new Arkade.Core.Base.Addml.Record(recordDefinition, new List<Field> {
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