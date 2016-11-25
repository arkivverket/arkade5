using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
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
                    throw new ArgumentException("Sum of field lengths (" + sumOfFieldLengths +
                                                ") does not match record length (" + _recordLength + ")");
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
        }

        public bool HasMoreRecords()
        {
            return !_streamReader.EndOfStream;
        }

        private char[] oldBuf;

        public Tuple<string, List<string>> GetNextValue()
        {
            int recordSeparatorLength = _recordSeparator != null ? _recordSeparator.GetLength() : 0;
            int fullLength = _recordLength + recordSeparatorLength;

            char[] buffer = new char[fullLength];
            int read = _streamReader.ReadBlock(buffer, 0, fullLength);

            if (read != fullLength)
            {
                throw new IOException("Unable to read a full record (including recordSeparator) of " + fullLength + " characters. Could only read " + read + " characters");
            }

            oldBuf = buffer;

            string recordString = new string(buffer);

            if (_recordSeparator != null && recordString.Substring(_recordLength, recordSeparatorLength) != _recordSeparator.Get())
            {
                throw new ArkadeException("RecordSeparator (" + _recordSeparator.ToString() + ") not found after record. Record with separator: '"+ StringUtil.WhiteSpaceToEscaped(recordString) +"'");
            }

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
        }
    }
}