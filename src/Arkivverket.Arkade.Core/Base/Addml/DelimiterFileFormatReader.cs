using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            string [] strings = JoinQuotedValues(Split(currentLine));

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

        private IEnumerable<string> Split(string stringToSplit)
        {
            var strings = new List<string>();
            var buffer = "";
            var fdIndex = 0;

            foreach (char c in stringToSplit)
            {
                buffer += c;

                if (c == _fieldDelimiter[fdIndex])
                {
                    fdIndex++;

                    if (fdIndex != _fieldDelimiter.Length)
                        continue;
                    
                    strings.Add(buffer[..^_fieldDelimiter.Length]);
                    fdIndex = 0;
                    buffer = "";
                }
                else
                    fdIndex = 0;
            }

            strings.Add(buffer);

            return strings;
        }

        private string[] JoinQuotedValues(IEnumerable<string> splitFieldValues)
        {
            if (_quotingChar == null)
                return splitFieldValues.ToArray();

            var fieldValues = new List<string>();
            var fieldValue = "";
            var concatenating = false;
            foreach (string splitFieldValue in splitFieldValues)
            {
                if (concatenating)
                {
                    fieldValue += _fieldDelimiter;

                    if (EndsWithOddNumberOfQuotingChars(splitFieldValue))
                    {
                        fieldValue += TrimQuotingCharFromEnd(splitFieldValue);
                        fieldValues.Add(fieldValue);
                        fieldValue = "";
                        concatenating = false;
                    }
                    else
                        fieldValue += splitFieldValue;
                    
                }
                else
                {
                    if (splitFieldValue.StartsWith(_quotingChar))
                    {
                        if (EndsWithOddNumberOfQuotingChars(splitFieldValue))
                            fieldValues.Add(TrimQuotingChar(splitFieldValue));
                        else
                        {
                            fieldValue = TrimQuotingCharFromStart(splitFieldValue);
                            concatenating = true;
                        }
                    }
                    else
                        fieldValues.Add(splitFieldValue);
                }
            }

            return fieldValues.ToArray();
        }

        private string TrimQuotingChar(string valueWithQuotingChar)
        {
            return valueWithQuotingChar[_quotingChar.Length..^_quotingChar.Length];
        }

        private string TrimQuotingCharFromStart(string valueWithQuotingChar)
        {
            return valueWithQuotingChar[_quotingChar.Length..];
        }

        private string TrimQuotingCharFromEnd(string valueWithQuotingChar)
        {
            return valueWithQuotingChar[..^_quotingChar.Length];
        }

        private bool EndsWithOddNumberOfQuotingChars(string value)
        {
            var numberOfQuotingChars = 0;
            string copy = value;
            while (copy.EndsWith(_quotingChar))
            {
                numberOfQuotingChars++;
                copy = TrimQuotingCharFromEnd(copy);
            }

            return numberOfQuotingChars % 2 == 1;
        }
    }
}