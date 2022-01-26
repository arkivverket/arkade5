using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml
{

    // TODO jostein: FixedFileFormatReader and FixedFormatReader should be merged together
    public class FixedFormatReader
    {
        private readonly Dictionary<string, List<int>> _fieldLengthsPerRecordDefinition;
        private readonly int? _identifierLength;
        private readonly int? _identifierStartPosition;
        private readonly int _recordLength;
        private readonly Separator _recordSeparator;
        private readonly StreamReader _streamReader;
        public long RecordNumber;

        public FixedFormatReader(StreamReader streamReader, FixedFormatConfig fixedFormatConfig)
        {
            _streamReader = streamReader;
            _recordLength = fixedFormatConfig.RecordLength;
            _recordSeparator = fixedFormatConfig.RecordSparator;

            if (fixedFormatConfig.RecordDefinitions.Count < 1)
            {
                throw new ArgumentException(
                    "fixedFormatDefinition.RecordDefinitions must contain at least 1 FixedFormatRecordDefinition");
            }

            // If only one recordDefinition and recordIdentifier is not set, set it to empty string
            if (fixedFormatConfig.RecordDefinitions.Count == 1 && fixedFormatConfig.RecordDefinitions[0].RecordIdentifier == null)
            {
                fixedFormatConfig.RecordDefinitions[0].RecordIdentifier = "";
            }


            _fieldLengthsPerRecordDefinition = new Dictionary<string, List<int>>();
            foreach (FixedFormatRecordConfig f in fixedFormatConfig.RecordDefinitions)
            {
                List<int> fieldLengths = f.FieldLengths;

                _fieldLengthsPerRecordDefinition.Add(f.RecordIdentifier, fieldLengths);

                int sumOfFieldLengths = fieldLengths.Sum();
                if (sumOfFieldLengths != _recordLength)
                {
                    var message = string.Format(Resources.Messages.ExceptionFixedLengthSumIsNotCorrect, 
                        AddmlLocation.FromRecordIndex(f.RecordIndex), _recordLength, sumOfFieldLengths);
                    throw new ArkadeException(message);
                }
            }

            _identifierStartPosition = fixedFormatConfig.IdentifierStartPosition;
            _identifierLength = fixedFormatConfig.IdentifierLength;

            if ((_identifierStartPosition.HasValue && !_identifierLength.HasValue) ||
                (!_identifierStartPosition.HasValue && _identifierLength.HasValue))
            {
                throw new ArgumentException("Both IdentifierStartPosition and IdentifierLength must be set");
            }

            if ((fixedFormatConfig.RecordDefinitions.Count <= 1) && _identifierStartPosition.HasValue &&
                _identifierLength.HasValue)
            {
                throw new ArgumentException(
                    "fixedFormatDefinition.RecordDefinitions must contain more than 1 FixedFormatRecordDefinition if identifier values are set");
            }

            RecordNumber = 0;
        }

        public bool HasMoreRecords()
        {
            return !_streamReader.EndOfStream;
        }

        public Tuple<string, List<string>> GetNextValue()
        {
            RecordNumber++;

            int recordSeparatorLength = _recordSeparator?.GetLength() ?? 0;
            int fullLength = _recordLength + recordSeparatorLength;

            var buffer = new char[fullLength];

            int read;
            string recordString;

            if (_recordSeparator != null)
            {
                recordString = _streamReader.ReadLine(_recordSeparator.Get());
                read = recordString.Length;
            }
            else
            {
                read = _streamReader.ReadBlock(buffer, 0, fullLength);
                recordString = new string(buffer);
            }

            if (read != fullLength)
                throw new ArkadeAddmlDelimiterException(string.Format(Resources.AddmlMessages.RecordFixedLengthMismatch,
                    fullLength, read), recordData: recordString, recordNumber: RecordNumber.ToString());

            List<int> fieldLengths;
            string recordIdentifier;
            if (_identifierStartPosition.HasValue && _identifierLength.HasValue)
            {
                recordIdentifier = recordString.Substring(_identifierStartPosition.Value, _identifierLength.Value);
                if (!_fieldLengthsPerRecordDefinition.ContainsKey(recordIdentifier))
                {
                    throw new Exception("No such recordIdentifier");
                }
                fieldLengths = _fieldLengthsPerRecordDefinition[recordIdentifier];
            }
            else
            {
                recordIdentifier = "";
                fieldLengths = _fieldLengthsPerRecordDefinition.Values.First();
            }

            List<string> fields = GetFields(recordString, fieldLengths);

            return new Tuple<string, List<string>>(recordIdentifier, fields);
        }


        private List<string> GetFields(string recordString, List<int> fieldLengths)
        {
            List<string> fields = new List<string>();
            int offset = 0;
            foreach (int fieldLength in fieldLengths)
            {
                string fieldString = recordString.Substring(offset, fieldLength);
                offset += fieldLength;
                fields.Add(fieldString);
            }
            return fields;
        }

        public class FixedFormatConfig
        {
            public int? IdentifierStartPosition;
            public int? IdentifierLength;
            public int RecordLength; // Support for different records lengths per record definition?
            public List<FixedFormatRecordConfig> RecordDefinitions;
            public Separator RecordSparator;
        }

        public class FixedFormatRecordConfig
        {
            public string RecordIdentifier;
            public List<int> FieldLengths;
            public RecordIndex RecordIndex;
        }
    }
}