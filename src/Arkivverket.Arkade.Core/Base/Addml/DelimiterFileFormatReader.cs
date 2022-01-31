using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class DelimiterFileFormatReader : FileFormatReader
    {
        private readonly string _fieldDelimiter;
        private readonly string _quotingChar;
        private readonly IEnumerator<string> _lines;

        // Zero based position of field used to identify recordDefinition
        private readonly int? _recordIdentifierPosition;

        public override Record Current => GetCurrentRecord();
        private int? _currentHeaderLevel;
        private string _currentName;

        public DelimiterFileFormatReader(FlatFile flatFile) : this(flatFile, GetStream(flatFile))
        {
        }

        public DelimiterFileFormatReader(FlatFile flatFile, StreamReader streamReader) : base(flatFile.Definition)
        {
            string recordDelimiter = GetRecordDelimiter(flatFile);
            _fieldDelimiter = GetFieldDelimiter(flatFile);
            _quotingChar = flatFile.Definition.QuotingChar;
            if (_quotingChar != null && _fieldDelimiter.Equals(_quotingChar))
                throw new ArkadeAddmlDelimiterException(string.Format(
                    Resources.AddmlMessages.FieldDelimiterAndQuotingCharCannotHaveSameValue, _quotingChar,
                    _fieldDelimiter));
            _recordIdentifierPosition = flatFile.GetRecordIdentifierPosition();
            _lines = new DelimiterFileRecordEnumerable(streamReader, recordDelimiter).GetEnumerator();
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

            string [] strings = SplitAndTrimCurrentLineToCleanStringValues(currentLine);

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
                    fielddata);
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
                _currentHeaderLevel = recordDefinition.HeaderLevel;
                RecordNumber = 0;
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

            while (RecordNumber < _currentHeaderLevel)
            {
                if (!_lines.MoveNext())
                    return false;
                RecordNumber++;
            }

            RecordNumber++;
            return true;
        }

        public override void Reset()
        {
            _lines.Reset();
        }

        private string[] SplitAndTrimCurrentLineToCleanStringValues(string stringToSplit)
        {
            var strings = new List<string>();
            var buffer = "";
            var qcIndex = 0;
            var fdIndex = 0;
            var quotingCharsFoundSincePreviousFieldDelimiter = 0;

            foreach (char c in stringToSplit)
            {
                buffer += c;

                if (c == _fieldDelimiter[fdIndex] && quotingCharsFoundSincePreviousFieldDelimiter != 1)
                {
                    fdIndex++;
                    if (fdIndex == _fieldDelimiter.Length)
                    {
                        strings.Add(quotingCharsFoundSincePreviousFieldDelimiter == 0 || _quotingChar == null
                            ? buffer[Range.EndAt(Index.FromEnd(_fieldDelimiter.Length))]
                            : buffer.Substring(_quotingChar.Length, buffer.Length - (_fieldDelimiter.Length + 2*_quotingChar.Length)));
                        fdIndex = 0;
                        buffer = "";
                        quotingCharsFoundSincePreviousFieldDelimiter = 0;
                    }
                }
                else
                    fdIndex = 0;

                if (_quotingChar != null && c == _quotingChar[qcIndex])
                {
                    qcIndex++;
                    if (qcIndex == _quotingChar.Length)
                    {
                        quotingCharsFoundSincePreviousFieldDelimiter++;
                        qcIndex = 0;
                    }
                }
                else
                    qcIndex = 0;
            }

            strings.Add(quotingCharsFoundSincePreviousFieldDelimiter == 0 || _quotingChar == null
                ? buffer
                : buffer.Substring(_quotingChar.Length, buffer.Length - 2*_quotingChar.Length));

            return strings.ToArray();
        }
    }
}