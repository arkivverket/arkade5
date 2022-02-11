using System.IO;
using System;
using System.Text;
using Arkivverket.Arkade.Core.Base;
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
                .WithRecordSeparator("CRLF")
                .WithFieldSeparator(";")
                .WithQuotingChar("\"")
                .Build();

            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

            const string csvData = "AA;\"B;B\";CC;\"DD\"";

            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
            var recordReader = new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader);
            var actionOfGettingCurrent = (Action) (() => ((Func<object>) (() => recordReader.Current))());

            recordReader.MoveNext(); // AA;"B;B";CC;"DD"

            actionOfGettingCurrent.Should().NotThrow<Exception>();
            recordReader.Current?.Fields?.Count.Should().Be(4);
        }

        [Fact]
        public void FieldDelimitersWithinQuotingCharsAreNotInterpretedAsFieldDelimiters()
        {
            string[] quotingStrings = {"\"", "\"\"\"", "|", ";", "*", "#", "|s", "as\\d5", "\\*+?|{[()^$.#"};

            foreach (string quotingString in quotingStrings)
            {
                AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                    .WithRecordSeparator("CRLF")
                    .WithFieldSeparator(",")
                    .WithQuotingChar(quotingString)
                    .Build();

                AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                    .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

                new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
                new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

                var csvData = $"{quotingString}B,B{quotingString},{quotingString}1234,56{quotingString}";

                var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
                var recordReader = new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader);
                var actionOfGettingCurrent = (Action)(() => ((Func<object>)(() => recordReader.Current))());

                recordReader.MoveNext();

                actionOfGettingCurrent.Should().NotThrow<Exception>();
                recordReader.Current?.Fields?.Count.Should().Be(2);
                recordReader.Current?.Fields?[0].Value.Should().Be("B,B");
                recordReader.Current?.Fields?[1].Value.Should().Be("1234,56");
            }
        }

        [Fact]
        public void QuotingCharWithinQuotingCharsAreNotInterpretedAsQuotingChar()
        {
            string[] quotingStrings = { "\"", "\"\"\"", "|", ";", "*", "#", "|s", "as\\d5", "\\*+?|{[()^$.#" };

            foreach (string quotingString in quotingStrings)
            {
                AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                    .WithRecordSeparator("CRLF")
                    .WithFieldSeparator(",")
                    .WithQuotingChar(quotingString)
                    .Build();

                AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                    .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

                new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

                var csvData = $"{quotingString}A{quotingString}B{quotingString}";

                var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
                var recordReader = new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader);
                var actionOfGettingCurrent = (Action)(() => ((Func<object>)(() => recordReader.Current))());

                recordReader.MoveNext();

                actionOfGettingCurrent.Should().NotThrow<Exception>();
                recordReader.Current?.Fields?.Count.Should().Be(1);
                recordReader.Current?.Fields?[0].Value.Should().Be($"A{quotingString}B");
            }
        }

        [Fact]
        public void FieldIsOnlyQuotedWhenStartingAndEndingWithQuotingChar()
        {
            string[] quotingStrings = { "\"", "\"\"\""};
            string[] fieldSeparators = { ";", "<=>"};

            foreach (string quotingString in quotingStrings)
            {
                foreach (string fieldSeparator in fieldSeparators)
                {
                    AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                        .WithRecordSeparator("CRLF")
                        .WithFieldSeparator(fieldSeparator)
                        .WithQuotingChar(quotingString)
                        .Build();

                    AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                        .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

                    new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
                    new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
                    new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
                    new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();
                    new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

                    string csvData = $"{quotingString}A{quotingString}B{quotingString}{fieldSeparator}" +
                                     $"C{quotingString}{fieldSeparator}" +
                                     $"{quotingString}D{quotingString}{quotingString}{fieldSeparator} asd{quotingString}{fieldSeparator}" +
                                     $"E{quotingString}noko{quotingString}{fieldSeparator}" +
                                     "F";

                    var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
                    var recordReader = new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader);
                    var actionOfGettingCurrent = (Action)(() => ((Func<object>)(() => recordReader.Current))());

                    recordReader.MoveNext();

                    actionOfGettingCurrent.Should().NotThrow<Exception>();
                    recordReader.Current?.Fields?.Count.Should().Be(5);
                    recordReader.Current?.Fields?[0].Value.Should().Be($"A{quotingString}B");
                    recordReader.Current?.Fields?[1].Value.Should().Be($"C{quotingString}");
                    recordReader.Current?.Fields?[2].Value.Should().Be($"D{quotingString}{quotingString}{fieldSeparator} asd");
                    recordReader.Current?.Fields?[3].Value.Should().Be($"E{quotingString}noko{quotingString}");
                    recordReader.Current?.Fields?[4].Value.Should().Be("F");
                }
            }
        }

        [Fact]
        public void QuotingCharEqualToFieldDelimiterShouldThrowException()
        {
        
            AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder()
                .WithRecordSeparator("CRLF")
                .WithFieldSeparator(",")
                .WithQuotingChar(",")
                .Build();

            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition).Build();

            new AddmlFieldDefinitionBuilder().WithRecordDefinition(recordDefinition).Build();

            const string csvData = ",A,B,";

            var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvData)));
            var actionOfCreatingRecordReader = (Action)(() => ((Func<object>)(() => new DelimiterFileFormatReader(new FlatFile(addmlFlatFileDefinition), streamReader)))());

            actionOfCreatingRecordReader.Should().Throw<ArkadeAddmlDelimiterException>();
        }
    }
}
