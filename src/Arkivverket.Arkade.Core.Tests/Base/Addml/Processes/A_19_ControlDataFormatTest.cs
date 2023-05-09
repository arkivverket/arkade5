using System.Collections.Generic;
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
            AddmlFieldDefinition fieldDefinition4 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n,nnn", null))
                .Build();
            AddmlFieldDefinition fieldDefinition5 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n nnn", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "1"), 1);
            test.Run(new Field(fieldDefinition1, "1.100"), 2);
            test.Run(new Field(fieldDefinition1, "1.1"), 3);
            test.Run(new Field(fieldDefinition1, "1E+2"), 4);
            test.Run(new Field(fieldDefinition1, "notanint1"), 5);
            test.Run(new Field(fieldDefinition2, "1"), 1);
            test.Run(new Field(fieldDefinition2, "1.100"), 2);
            test.Run(new Field(fieldDefinition2, "1.1"), 3);
            test.Run(new Field(fieldDefinition2, "1E+2"), 4);
            test.Run(new Field(fieldDefinition2, "notanint1"), 5);
            test.Run(new Field(fieldDefinition3, "1"), 1);
            test.Run(new Field(fieldDefinition3, "1.100"), 2);
            test.Run(new Field(fieldDefinition3, "1.1"), 3);
            test.Run(new Field(fieldDefinition3, "1E+2"), 4);
            test.Run(new Field(fieldDefinition3, "notanint1"), 5);
            test.Run(new Field(fieldDefinition4, "1"), 1);
            test.Run(new Field(fieldDefinition4, "1,100"), 2);
            test.Run(new Field(fieldDefinition4, "1,1"), 3);
            test.Run(new Field(fieldDefinition4, "1E+2"), 4);
            test.Run(new Field(fieldDefinition4, "notanint1"), 5);
            test.Run(new Field(fieldDefinition5, "1"), 1);
            test.Run(new Field(fieldDefinition5, "1 100"), 2);
            test.Run(new Field(fieldDefinition5, "1 1"), 3);
            test.Run(new Field(fieldDefinition5, "1E+2"), 4);
            test.Run(new Field(fieldDefinition5, "notanint1"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(5);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 3, 4, 5");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: '1.1', '1E+2', 'notanint1'");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 1, 2, 3, 5");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig dataformat: '1', '1.100', '1.1', 'notanint1'");
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Be($"{fieldDefinition3.GetIndex()} - linje(r): 2, 3, 4, 5");
            testRun.TestResults.TestsResults[2].Message.Should().Be("Ugyldig dataformat: '1.100', '1.1', '1E+2', 'notanint1'");
            testRun.TestResults.TestsResults[3].Location.ToString().Should().Be($"{fieldDefinition4.GetIndex()} - linje(r): 3, 4, 5");
            testRun.TestResults.TestsResults[3].Message.Should().Be("Ugyldig dataformat: '1,1', '1E+2', 'notanint1'");
            testRun.TestResults.TestsResults[4].Location.ToString().Should().Be($"{fieldDefinition5.GetIndex()} - linje(r): 4, 5");
            testRun.TestResults.TestsResults[4].Message.Should().Be("Ugyldig dataformat: '1E+2', 'notanint1'");
        }

        [Fact]
        public void ShouldReportFormatValidityForIntegerNullValuesCorrectly()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .NonNullable()
                .WithDataType(new IntegerDataType("n.nnn", new List<string> { "null", "" }))
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n.nnn", null))
                .NonNullable()
                .Build();
            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n.nnn", new List<string> { "null", "" }))
                .Build();
            AddmlFieldDefinition fieldDefinition4 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new IntegerDataType("n.nnn", null))
                .Build();

            FlatFile flatFile = new(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, "null"), 1);
            test.Run(new Field(fieldDefinition1, ""), 2);
            test.Run(new Field(fieldDefinition1, " "), 3);
            test.Run(new Field(fieldDefinition2, "null"), 1);
            test.Run(new Field(fieldDefinition2, ""), 2); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition2, " "), 3);
            test.Run(new Field(fieldDefinition3, "null"), 1);
            test.Run(new Field(fieldDefinition3, ""), 2); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition3, " "), 3);
            test.Run(new Field(fieldDefinition4, "null"), 1);
            test.Run(new Field(fieldDefinition4, ""), 2); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition4, " "), 3);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(4);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 3");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: ' '");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 1, 3");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig dataformat: 'null', ' '");
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Be($"{fieldDefinition3.GetIndex()} - linje(r): 3");
            testRun.TestResults.TestsResults[2].Message.Should().Be("Ugyldig dataformat: ' '");
            testRun.TestResults.TestsResults[3].Location.ToString().Should().Be($"{fieldDefinition4.GetIndex()} - linje(r): 1, 3");
            testRun.TestResults.TestsResults[3].Message.Should().Be("Ugyldig dataformat: 'null', ' '");
        }

        [Fact]
        public void ShouldReportIncorrectStringDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("fnr", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("org", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType("knr", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition4 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new StringDataType(null, null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""), 1);
            test.Run(new Field(fieldDefinition1, "17080232930"), 2); // not ok
            test.Run(new Field(fieldDefinition1, "17080232934"), 3); // ok
            test.Run(new Field(fieldDefinition2, ""), 1);
            test.Run(new Field(fieldDefinition2, "914994781"), 2); // not ok
            test.Run(new Field(fieldDefinition2, "914994780"), 3); // ok
            test.Run(new Field(fieldDefinition3, ""), 1);
            test.Run(new Field(fieldDefinition3, "63450608211"), 2); // not ok
            test.Run(new Field(fieldDefinition3, "63450608213"), 3); // ok
            test.Run(new Field(fieldDefinition4, ""), 1);
            test.Run(new Field(fieldDefinition4, "A"), 2);
            test.Run(new Field(fieldDefinition4, "B"), 3);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(3);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 2");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: '17080232930'");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 2");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig dataformat: '914994781'");
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Be($"{fieldDefinition3.GetIndex()} - linje(r): 2");
            testRun.TestResults.TestsResults[2].Message.Should().Be("Ugyldig dataformat: '63450608211'");
        }

        [Fact]
        public void ShouldReportIncorrectFloatDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("nn,nn", null))
                .NonNullable()
                .Build();
            
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n.nnn,nn", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("nn.nn", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition4 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n nnn,nn", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition5 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n,nnn.nn", null))
                .NonNullable()
                .Build();

            AddmlFieldDefinition fieldDefinition6 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n nnn.nn", null))
                .NonNullable()
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""), 1);
            test.Run(new Field(fieldDefinition1, "1"), 2);
            test.Run(new Field(fieldDefinition1, "1,2"), 3);
            test.Run(new Field(fieldDefinition1, "1.200"), 4);
            test.Run(new Field(fieldDefinition1, "1.200,3"), 5);
            test.Run(new Field(fieldDefinition1, "not"), 6);
            test.Run(new Field(fieldDefinition2, ""), 1);
            test.Run(new Field(fieldDefinition2, "1"), 2);
            test.Run(new Field(fieldDefinition2, "1,2"), 3);
            test.Run(new Field(fieldDefinition2, "1.200,3"), 4);
            test.Run(new Field(fieldDefinition2, "not"), 5);
            test.Run(new Field(fieldDefinition3, ""), 1);
            test.Run(new Field(fieldDefinition3, "1"), 2);
            test.Run(new Field(fieldDefinition3, "1.2"), 3);
            test.Run(new Field(fieldDefinition3, "1,200"), 4);
            test.Run(new Field(fieldDefinition3, "1,200.3"), 5);
            test.Run(new Field(fieldDefinition3, "not"), 6);
            test.Run(new Field(fieldDefinition4, ""), 1);
            test.Run(new Field(fieldDefinition4, "1"), 2);
            test.Run(new Field(fieldDefinition4, "1,2"), 3);
            test.Run(new Field(fieldDefinition4, "1 200,3"), 4);
            test.Run(new Field(fieldDefinition4, "not"), 5);
            test.Run(new Field(fieldDefinition5, ""), 1);
            test.Run(new Field(fieldDefinition5, "1"), 2);
            test.Run(new Field(fieldDefinition5, "1.2"), 3);
            test.Run(new Field(fieldDefinition5, "1,200.3"), 4);
            test.Run(new Field(fieldDefinition5, "not"), 5);
            test.Run(new Field(fieldDefinition6, ""), 1);
            test.Run(new Field(fieldDefinition6, "1"), 2);
            test.Run(new Field(fieldDefinition6, "1.2"), 3);
            test.Run(new Field(fieldDefinition6, "1 200.3"), 4);
            test.Run(new Field(fieldDefinition6, "not"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(6);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 4, 5, 6");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: '1.200', '1.200,3', 'not'");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig dataformat: 'not'");
            testRun.TestResults.TestsResults[2].Location.ToString().Should().Be($"{fieldDefinition3.GetIndex()} - linje(r): 4, 5, 6");
            testRun.TestResults.TestsResults[2].Message.Should().Be("Ugyldig dataformat: '1,200', '1,200.3', 'not'");
            testRun.TestResults.TestsResults[3].Location.ToString().Should().Be($"{fieldDefinition4.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[3].Message.Should().Be("Ugyldig dataformat: 'not'");
            testRun.TestResults.TestsResults[4].Location.ToString().Should().Be($"{fieldDefinition5.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[4].Message.Should().Be("Ugyldig dataformat: 'not'");
            testRun.TestResults.TestsResults[5].Location.ToString().Should().Be($"{fieldDefinition6.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[5].Message.Should().Be("Ugyldig dataformat: 'not'");
        }

        [Fact]
        public void ShouldReportIncorrectDateDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null))
                .NonNullable()
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""), 1); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition1, "18.11.2016T08:43:00+00:00"), 2); // ok
            test.Run(new Field(fieldDefinition1, "notadate"), 3);
            test.Run(new Field(fieldDefinition1, "40.11.2016T08:43:00+00:00"), 4);
            test.Run(new Field(fieldDefinition1, "18.11.2016"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 3, 4, 5");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: 'notadate', '40.11.2016T08:43:00+00:00', '18.11.2016'");
        }

        [Fact]
        public void ShouldReportIncorrectBooleanDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Y/N", null))
                .NonNullable()
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new BooleanDataType("Ja/Nei", null))
                .NonNullable()
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""), 1); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition1, "Y"), 2);
            test.Run(new Field(fieldDefinition1, "N"), 3);
            test.Run(new Field(fieldDefinition1, "Ja"), 4);
            test.Run(new Field(fieldDefinition1, "Nei"), 5);
            test.Run(new Field(fieldDefinition2, ""), 1); // empty value is handled by A.20
            test.Run(new Field(fieldDefinition2, "Y"), 2);
            test.Run(new Field(fieldDefinition2, "N"), 3);
            test.Run(new Field(fieldDefinition2, "Ja"), 4);
            test.Run(new Field(fieldDefinition2, "Nei"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(2);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 4, 5");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: 'Ja', 'Nei'");
            testRun.TestResults.TestsResults[1].Location.ToString().Should().Be($"{fieldDefinition2.GetIndex()} - linje(r): 2, 3");
            testRun.TestResults.TestsResults[1].Message.Should().Be("Ugyldig dataformat: 'Y', 'N'");
        }

        [Fact]
        public void ShouldReportIncorrectLinkDataFormat()
        {
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithDataType(new LinkDataType("url", null))
                .Build();

            FlatFile flatFile = new FlatFile(fieldDefinition1.GetAddmlFlatFileDefinition());

            A_19_ControlDataFormat test = new A_19_ControlDataFormat();
            test.Run(flatFile);
            test.Run(new Field(fieldDefinition1, ""), 1);
            test.Run(new Field(fieldDefinition1, "http://en.url.her"), 2);
            test.Run(new Field(fieldDefinition1, "https://en.url.her"), 3);
            test.Run(new Field(fieldDefinition1, "www.webpage.net"), 4);
            test.Run(new Field(fieldDefinition1, "not-a-url"), 5);
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 5");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: 'not-a-url'");
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
            test.Run(new Field(fieldDefinition1, "19980803"), 1); // not ok
            test.Run(new Field(fieldDefinition1, "19990715"), 2); // not ok
            test.Run(new Field(fieldDefinition1, "19880805"), 3); // not ok
            test.Run(new Field(fieldDefinition1, "19890915"), 4); // not ok
            test.Run(new Field(fieldDefinition1, "19990803"), 5); // not ok
            test.Run(new Field(fieldDefinition1, "19880211"), 6); // not ok
            test.Run(new Field(fieldDefinition1, "19890311"), 7); // not ok, not shown
            test.Run(new Field(fieldDefinition1, "19890725"), 8); // not ok, not shown
            test.EndOfFile();

            TestRun testRun = test.GetTestRun();
            testRun.IsSuccess().Should().BeFalse();
            testRun.TestResults.GetNumberOfResults().Should().Be(1);
            testRun.TestResults.TestsResults[0].Location.ToString().Should().Be($"{fieldDefinition1.GetIndex()} - linje(r): 1, 2, 3, 4, 5, 6, 7, 8");
            testRun.TestResults.TestsResults[0].Message.Should().Be("Ugyldig dataformat: '19980803', '19990715', '19880805', '19890915', '19990803', '19880211' + 2 flere");
        }
    }
}
