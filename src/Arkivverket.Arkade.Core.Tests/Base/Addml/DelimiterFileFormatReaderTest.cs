using System.IO;
using System;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Base.Addml.Record;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class DelimiterFileFormatReaderTest
    {
        [Fact]
        public void ShouldSupportReadingOfMultipleRecordTypesWithinSameFlatFile()
        {
            var fieldPosttype = "posttype";
            var fieldPostnummer = "postnummer";
            var fieldFirmanavn = "firmanavn";
            var fieldPersonnavn = "personnavn";

            AddmlFlatFileDefinition fileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithRecordSeparator("CRLF")
                .WithFieldSeparator(";")
                .WithRecordDefinitionFieldIdentifier(fieldPosttype)
                .Build();

            var recdef1 = "recdef1";
            AddmlRecordDefinition recordDefinition1 = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(fileDefinition)
                .WithName(recdef1)
                .WithRecordDefinitionFieldValue("1")
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition1)
                .WithName(fieldPosttype)
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition1)
                .WithName(fieldPostnummer)
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition1)
                .WithName(fieldFirmanavn)
                .Build();

            var recdef2 = "recdef2";
            AddmlRecordDefinition recordDefinition2 = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(fileDefinition)
                .WithName(recdef2)
                .WithRecordDefinitionFieldValue("2")
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition2)
                .WithName(fieldPosttype)
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition2)
                .WithName(fieldPersonnavn)
                .Build();
            new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(recordDefinition2)
                .WithName(fieldPostnummer)
                .Build();

            var sb = new StringBuilder();
            sb.Append("1;2500;Skattedirektoratet\r\n");
            sb.Append("1;2501;Skattedirektoratet\r\n");
            sb.Append("2;Hans Hansen;3374\r\n");
            sb.Append("2;Ole Olsen;2235\r\n");

            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));

            var reader = new DelimiterFileFormatReader(new FlatFile(fileDefinition), streamReader);
            reader.MoveNext().Should().BeTrue();
            reader.Current.Definition.Name.Should().Be(recdef1);
            RecordShouldMatch(reader.Current, 0, fieldPosttype, "1");
            RecordShouldMatch(reader.Current, 1, fieldPostnummer, "2500");
            RecordShouldMatch(reader.Current, 2, fieldFirmanavn, "Skattedirektoratet");

            reader.MoveNext().Should().BeTrue();
            reader.Current.Definition.Name.Should().Be(recdef1);
            RecordShouldMatch(reader.Current, 0, fieldPosttype, "1");
            RecordShouldMatch(reader.Current, 1, fieldPostnummer, "2501");
            RecordShouldMatch(reader.Current, 2, fieldFirmanavn, "Skattedirektoratet");

            reader.MoveNext().Should().BeTrue();
            reader.Current.Definition.Name.Should().Be(recdef2);
            RecordShouldMatch(reader.Current, 0, fieldPosttype, "2");
            RecordShouldMatch(reader.Current, 1, fieldPersonnavn, "Hans Hansen");
            RecordShouldMatch(reader.Current, 2, fieldPostnummer, "3374");

            reader.MoveNext().Should().BeTrue();
            reader.Current.Definition.Name.Should().Be(recdef2);
            RecordShouldMatch(reader.Current, 0, fieldPosttype, "2");
            RecordShouldMatch(reader.Current, 1, fieldPersonnavn, "Ole Olsen");
            RecordShouldMatch(reader.Current, 2, fieldPostnummer, "2235");

            reader.MoveNext().Should().BeFalse();
        }

        private void RecordShouldMatch(Record record, int fieldIndex, string name, string value)
        {
            record.Fields[fieldIndex].GetName().Should().Be(name);
            record.Fields[fieldIndex].Value.Should().Be(value);
        } 

        [Fact]
        public void SemicolonsWithinQuotesAreNotInterpretedAsFieldDelimiters()
        {
            AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithRecordSeparator("CRLF").WithFieldSeparator(";").Build();

            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

            const string csvData = "AA;\"B;B\";CC";

            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
            var recordReader = new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader);
            var actionOfGettingCurrent = (Action) (() => ((Func<object>) (() => recordReader.Current))());

            recordReader.MoveNext(); // AA;"B;B";CC

            actionOfGettingCurrent.Should().NotThrow<Exception>();
            recordReader.Current?.Fields?.Count.Should().Be(3);
        }
    }
}
