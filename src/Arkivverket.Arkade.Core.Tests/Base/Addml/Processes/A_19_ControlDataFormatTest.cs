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
    public class A_19_ControlDataFormatTest : LanguageDependentTest
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

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
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

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
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

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
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

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
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

        [Fact]
        public void ShouldReportIncorrectBooleanDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Y/N", null))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Ja/Nei", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""));
            test.Run(new Field(fieldDefinition1, "Y"));
            test.Run(new Field(fieldDefinition1, "N"));
            test.Run(new Field(fieldDefinition1, "Ja"));
            test.Run(new Field(fieldDefinition1, "Nei"));
            test.Run(new Field(fieldDefinition2, ""));
            test.Run(new Field(fieldDefinition2, "Y"));
            test.Run(new Field(fieldDefinition2, "N"));
            test.Run(new Field(fieldDefinition2, "Ja"));
            test.Run(new Field(fieldDefinition2, "Nei"));
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(2);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '', 'Ja', 'Nei'");
            testRun.Results[1].Location.ToString().Should().Be(fieldDefinition2.GetIndex().ToString());
            testRun.Results[1].Message.Should().Be("Ugyldig dataformat: '', 'Y', 'N'");
        }

        [Fact]
        public void ShouldOnlyShowSixErrorsPerFieldDefinition()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("fnr", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "19980803")); // not ok
            test.Run(new Field(fieldDefinition1, "19990715")); // not ok
            test.Run(new Field(fieldDefinition1, "19880805")); // not ok
            test.Run(new Field(fieldDefinition1, "19890915")); // not ok
            test.Run(new Field(fieldDefinition1, "19990803")); // not ok
            test.Run(new Field(fieldDefinition1, "19880211")); // not ok
            test.Run(new Field(fieldDefinition1, "19890311")); // not ok, not shown
            test.Run(new Field(fieldDefinition1, "19890725")); // not ok, not shown
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.Results.Count.Should().Be(1);
            testRun.Results[0].Location.ToString().Should().Be(fieldDefinition1.GetIndex().ToString());
            testRun.Results[0].Message.Should().Be("Ugyldig dataformat: '19980803', '19990715', '19880805', '19890915', '19990803', '19880211' + 2 flere");
        }
    }
}
