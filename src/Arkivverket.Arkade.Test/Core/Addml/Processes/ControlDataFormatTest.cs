using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Processes
{
    public class ControlDataFormatTest
    {

        [Fact]
        public void ShouldReportIncorrectIntegerDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n.nnn", null))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("nnE+exp", null))
                .Build();
            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType(null, null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "1"));
            test.Run(new Field(fieldDefinition1, "1.100"));
            test.Run(new Field(fieldDefinition1, "1.1"));
            test.Run(new Field(fieldDefinition1, "1E+2"));
            test.Run(new Field(fieldDefinition1, "notanint1"));
            test.Run(new Field(fieldDefinition2, "1"));
            test.Run(new Field(fieldDefinition2, "1.100"));
            test.Run(new Field(fieldDefinition2, "1.1"));
            test.Run(new Field(fieldDefinition2, "1E+2"));
            test.Run(new Field(fieldDefinition2, "notanint1"));
            test.Run(new Field(fieldDefinition3, "1"));
            test.Run(new Field(fieldDefinition3, "1.100"));
            test.Run(new Field(fieldDefinition3, "1.1"));
            test.Run(new Field(fieldDefinition3, "1E+2"));
            test.Run(new Field(fieldDefinition3, "notanint1"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '1.1', '1E+2', 'notanint1'");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition2.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig dataformat: '1', '1.100', '1.1', 'notanint1'");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition3.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("Ugyldig dataformat: '1.100', '1.1', '1E+2', 'notanint1'");
        }

        [Fact]
        public void ShouldReportIncorrectStringDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("fnr", null))
                .Build();

            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("org", null))
                .Build();

            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("knr", null))
                .Build();

            AddmlFieldDefinition fieldDefinition4 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType(null, null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, "17080232930")); // not ok
            test.Run(new Field(fieldDefinition1, "17080232934")); // ok
            test.Run(new Field(fieldDefinition2, ""));
            test.Run(new Field(fieldDefinition2, "914994781")); // not ok
            test.Run(new Field(fieldDefinition2, "914994780")); // ok
            test.Run(new Field(fieldDefinition3, ""));
            test.Run(new Field(fieldDefinition3, "63450608211")); // not ok
            test.Run(new Field(fieldDefinition3, "63450608213")); // ok
            test.Run(new Field(fieldDefinition4, ""));
            test.Run(new Field(fieldDefinition4, "A"));
            test.Run(new Field(fieldDefinition4, "B"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(3);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '', '17080232930'");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition2.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig dataformat: '', '914994781'");
            testRun.Results[2].Location.ToString().Should().Be(fieldDefinition3.GetIndex().ToString());
            testRun.Results[2].Message.Should().Be("Ugyldig dataformat: '', '63450608211'");
        }

        [Fact]
        public void ShouldReportIncorrectFloatDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("nn,nn", null))
                .Build();

            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n.nnn,nn", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, "1"));
            test.Run(new Field(fieldDefinition1, "1,2"));
            test.Run(new Field(fieldDefinition1, "1.200"));
            test.Run(new Field(fieldDefinition1, "1.200,3"));
            test.Run(new Field(fieldDefinition1, "not"));
            test.Run(new Field(fieldDefinition2, ""));
            test.Run(new Field(fieldDefinition2, "1"));
            test.Run(new Field(fieldDefinition2, "1,2"));
            test.Run(new Field(fieldDefinition2, "1.200,3"));
            test.Run(new Field(fieldDefinition2, "not"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(2);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '', '1.200', '1.200,3', 'not'");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition2.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig dataformat: '', 'not'");
        }

        [Fact]
        public void ShouldReportIncorrectDateDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, "18.11.2016T08:43:00+00:00")); // ok
            test.Run(new Field(fieldDefinition1, "notadate"));
            test.Run(new Field(fieldDefinition1, "40.11.2016T08:43:00+00:00"));
            test.Run(new Field(fieldDefinition1, "18.11.2016"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '', 'notadate', '40.11.2016T08:43:00+00:00', '18.11.2016'");
        }

        [Fact(Skip = "TODO jostein")]
        public void ShouldReportIncorrectBooleanDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType(null, null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, ""));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("");
        }

        [Fact(Skip = "TODO jostein")]
        public void ShouldReportIncorrectLinkDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new LinkDataType())
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            ControlDataFormat test = new ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, ""));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("");
        }
    }
}