using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml
{
    public class DelimiterBasedFileReader : FileReader
    {
        private readonly IEnumerator<string> _lines;
        private readonly string _fieldDelimiter;

        private bool _hasMoreRecords = true;

        // Zero based position of field used to identify recordDefinition
        private readonly int? _recordIdentifierPosition;

        public DelimiterBasedFileReader(FlatFile flatFile) : base(flatFile.Definition)
        {
            StreamReader stream = GetStream(flatFile);
            string recordDelimiter = GetRecordDelimiter(flatFile);
            _fieldDelimiter = GetFieldDelimiter(flatFile);
            _recordIdentifierPosition = GetRecordIdentifierPosition(flatFile);
            _lines = stream.ReadUntil(recordDelimiter).GetEnumerator();
        }

        private int? GetRecordIdentifierPosition(FlatFile flatFile)
        {
            // TODO!

            return null;
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


        public override bool HasMoreRecords()
        {
            return _hasMoreRecords;
        }

        public override Record GetNextRecord()
        {
            if (!_lines.MoveNext())
            {
                _hasMoreRecords = false;
            }

            List<Field> fields = new List<Field>();

            string currentLine = _lines.Current;

            string[] strings = currentLine.Split(new[] { _fieldDelimiter }, StringSplitOptions.None);

            string recordIdentifier = null;
            if (_recordIdentifierPosition.HasValue)
            {
                recordIdentifier = strings[_recordIdentifierPosition.Value];
            }
            AddmlRecordDefinition recordDefinition = GetAddmlRecordDefinition(recordIdentifier);
            List<AddmlFieldDefinition> fieldDefinitions = recordDefinition.AddmlFieldDefinitions;

            if (fieldDefinitions.Count != strings.Length)
            {
                throw new ArkadeException("Number of fields in record is not according to ADDML. Was " + strings.Length + ". Expected "+ fieldDefinitions.Count + ".");
            }

            for (int i = 0; i < strings.Length; i++)
            {
                string s = strings[i];
                AddmlFieldDefinition fieldDefinition = fieldDefinitions[i];
                fields.Add(new Field(fieldDefinition, s));
            }

            return new Record(recordDefinition, fields);
        }
    }

}
