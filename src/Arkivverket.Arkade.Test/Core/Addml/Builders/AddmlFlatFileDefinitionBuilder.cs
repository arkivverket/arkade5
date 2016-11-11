using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Test.Core.Addml.Builders
{
    internal class AddmlFlatFileDefinitionBuilder
    {
        private readonly string _charset = "UTF-8";
        private readonly FileInfo _fileInfo = null;
        private string _fileName = "";
        private readonly string _name = "FlatFileDefinition dummy name";

        private int? _numberOfRecords = null;
        private readonly List<string> _processes = null;
        private readonly string _recordDefinitionFieldIdentifier = null;
        private readonly string _recordSeparator = null;
        private readonly string _fieldSeparator = null;
        private readonly AddmlFlatFileFormat _format = AddmlFlatFileFormat.Fixed;

        public AddmlFlatFileDefinition Build()
        {
            return new AddmlFlatFileDefinition(_name, _fileName, _fileInfo, _recordSeparator, _fieldSeparator, _charset,
                _recordDefinitionFieldIdentifier, _numberOfRecords, _format, _processes);
        }

        public AddmlFlatFileDefinitionBuilder WithNumberOfRecords(int numberOfRecords)
        {
            _numberOfRecords = numberOfRecords;
            return this;
        }

        public AddmlFlatFileDefinitionBuilder WithFileName(string fileName)
        {
            _fileName = fileName;
            return this;
        }
    }
}