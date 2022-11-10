using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class DelimiterFileFormatReader : FileFormatReader
    {
        private readonly string _recordDelimiter;
        private readonly string _fieldDelimiter;
        private readonly string _quotingChar;
        private readonly IEnumerator<string> _lines;

        // Zero based position of field used to identify recordDefinition
        private readonly int? _recordIdentifierPosition;

        public override Record Current => GetCurrentRecord();
        private string _currentName;

        public DelimiterFileFormatReader(FlatFile flatFile) : this(flatFile, GetStream(flatFile))
        {
        }

        public DelimiterFileFormatReader(FlatFile flatFile, StreamReader streamReader) : base(flatFile.Definition)
        {
            _recordDelimiter = GetRecordDelimiter(flatFile);
            _fieldDelimiter = GetFieldDelimiter(flatFile);
            _quotingChar = flatFile.Definition.QuotingChar;
            if (_quotingChar != null && _fieldDelimiter.Equals(_quotingChar))
                throw new ArkadeAddmlDelimiterException(string.Format(
                    Resources.AddmlMessages.FieldDelimiterAndQuotingCharCannotHaveSameValue, _quotingChar,
                    _fieldDelimiter));
            _recordIdentifierPosition = flatFile.GetRecordIdentifierPosition();
            _lines = new DelimiterFileRecordEnumerable(streamReader, _recordDelimiter, _quotingChar).GetEnumerator();
        }

        private string GetFieldDelimiter(FlatFile flatFile)
        {
            return flatFile.Definition.FieldSeparator.Get();
        }

        private string GetRecordDelimiter(FlatFile flatFile)
        {
            return flatFile.Definition.RecordSeparator.Get();
        }

        private static StreamReader GetStream(FlatFile flatFile)
        {
            FileStream fileStream = flatFile.Definition.FileInfo.OpenRead();
            Encoding encoding = flatFile.Definition.Encoding;
            StreamReader streamReader = new StreamReader(fileStream, encoding);
            return streamReader;
        }

        private Record GetCurrentRecord()
        {
            List<Field> fields = new List<Field>();

            string currentLine = _lines.Current;

            string[] strings = Util.CsvHelper.Split(currentLine, _recordDelimiter, _fieldDelimiter, _quotingChar);
            

            string recordIdentifier = null;
            if (_recordIdentifierPosition.HasValue)
            {
                recordIdentifier = strings[_recordIdentifierPosition.Value];
            }
            AddmlRecordDefinition recordDefinition = GetAddmlRecordDefinition(recordIdentifier);
            List<AddmlFieldDefinition> fieldDefinitions = recordDefinition.AddmlFieldDefinitions;

            if (fieldDefinitions.Count != strings.Length)
            {
                int maxNumberOfCharactersInErrorMessage = 40;
                string fielddata = currentLine.Length <= maxNumberOfCharactersInErrorMessage ? currentLine : currentLine.Substring(0, maxNumberOfCharactersInErrorMessage-1);
                throw new ArkadeAddmlDelimiterException(
                    $"{Resources.AddmlMessages.UnexpectedNumberOfFields}: {strings.Length}/{fieldDefinitions.Count}",
                    recordDefinition.Name,
                    fielddata,
                    RecordNumber.ToString());
            }

            for (int i = 0; i < strings.Length; i++)
            {
                string s = strings[i];
                AddmlFieldDefinition fieldDefinition = fieldDefinitions[i];
                fields.Add(new Field(fieldDefinition, s));
            }

            if (_currentName != recordDefinition.Name)
            {
                _currentName = recordDefinition.Name;
                RecordNumber = 1;
            }

            return new Record(recordDefinition, RecordNumber, fields);
        }

        public override void Dispose()
        {
            _lines.Dispose();
        }

        public override bool MoveNext()
        {
            if (!_lines.MoveNext())
                return false;

            RecordNumber++;
            return true;
        }

        public override void Reset()
        {
            _lines.Reset();
        }
    }
}