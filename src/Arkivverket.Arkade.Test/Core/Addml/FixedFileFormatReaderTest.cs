using Arkivverket.Arkade.Core.Addml;
using System.Text;
using Xunit;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using System.IO;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class FixedFileFormatReaderTest
    {

        [Fact]
        public void ShouldUseRecordDefinitionFixedLengthAndSkipRecordSeparator()
        {
            AddmlFlatFileDefinition fileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithRecordSeparator("CRLF")
                .Build();
            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(fileDefinition)
                .WithRecordLength(7)
                .Build();
            AddmlFieldDefinition fieldDefinition1 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                .WithFixedLength(1)
                .Build();
            AddmlFieldDefinition fieldDefinition2 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                .WithFixedLength(2)
                .Build();
            AddmlFieldDefinition fieldDefinition3 = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition)
                 .WithFixedLength(4)
                .Build();

            FlatFile file = new FlatFile(fileDefinition);

            var sb = new StringBuilder();
            sb.Append("1121234\r\n");
            sb.Append("AABABCD\r\n");
            sb.Append("       \r\n");
            MemoryStream stream = CreateStream(sb.ToString());

            FixedFileFormatReader reader = new FixedFileFormatReader(file, stream);

            reader.MoveNext().Should().BeTrue();
            reader.Current.Value.Should().Be("1121234");
            reader.Current.Fields[0].Value.Should().Be("1");
            reader.Current.Fields[1].Value.Should().Be("12");
            reader.Current.Fields[2].Value.Should().Be("1234");
            reader.MoveNext().Should().BeTrue();
            reader.Current.Value.Should().Be("AABABCD");
            reader.Current.Fields[0].Value.Should().Be("A");
            reader.Current.Fields[1].Value.Should().Be("AB");
            reader.Current.Fields[2].Value.Should().Be("ABCD");
            reader.MoveNext().Should().BeTrue();
            reader.Current.Value.Should().Be("       ");
            reader.Current.Fields[0].Value.Should().Be(" ");
            reader.Current.Fields[1].Value.Should().Be("  ");
            reader.Current.Fields[2].Value.Should().Be("    ");
            reader.MoveNext().Should().BeFalse();
        }

        private MemoryStream CreateStream(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

    }
}
