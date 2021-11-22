using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class QuotingCharTest
    {
        [Fact]
        public void ShouldTrimQuotingCharFromValues()
        {
            var quotingChars = new[] {'"'.ToString(), "'", "*", "^", "|", ",", "#", "asd1'"};

            foreach (string quotingChar in quotingChars)
            {
                AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                    .WithQuotingChar(quotingChar)
                    .Build();

                AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                    .WithAddmlFlatFileDefinition(flatFileDefinition)
                    .Build();

                AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinitionBuilder()
                    .WithRecordDefinition(recordDefinition)
                    .WithDataType(new FloatDataType("n.nnn,nn", null))
                    .Build();

                const string value = "1.234,56";

                var field = new Field(addmlFieldDefinition, quotingChar + value + quotingChar);

                field.Value.Should().Be(value);
            }
        }

        [Fact]
        public void ShouldReturnCorrectValuesForUnquotedValues()
        {
            var quotingChars = new[] {'"'.ToString(), "'", "*", "^", "|", ",", "#", "asd2'"};

            foreach (string quotingChar in quotingChars)
            {
                AddmlFlatFileDefinition flatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                    .WithQuotingChar(quotingChar)
                    .Build();

                AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                    .WithAddmlFlatFileDefinition(flatFileDefinition)
                    .Build();

                AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinitionBuilder()
                    .WithRecordDefinition(recordDefinition)
                    .WithDataType(new FloatDataType("n.nnn,nn", null))
                    .Build();

                const string value = "1.234,56";

                var field = new Field(addmlFieldDefinition, value);
                var field2 = new Field(null, value);

                field.Value.Should().Be(value);
                field2.Value.Should().Be(value);
            }
        }

        [Fact]
        public void ShouldReturnValuesAsIsWhenQuotingCharIsNotSet()
        {
            AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinitionBuilder()
                .WithDataType(new FloatDataType("n.nnn,nn", null))
                .Build();

            const string value = @"'1.234,56'";

            var field = new Field(addmlFieldDefinition, value);
            var field2 = new Field(null, value);

            field.Value.Should().Be(value);
            field2.Value.Should().Be(value);
        }
    }
}