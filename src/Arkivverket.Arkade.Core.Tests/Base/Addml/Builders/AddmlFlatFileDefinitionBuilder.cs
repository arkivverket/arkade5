using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Builders
{
    internal class AddmlFlatFileDefinitionBuilder
    {
        private static readonly Random Random = new Random();

        private readonly string _charset = "UTF-8";
        private FileInfo _fileInfo = null;
        private string _fileName = "";
        private string _name = "FlatFile" + Random.Next();

        private int? _numberOfRecords = null;
        private readonly List<string> _processes = null;
        private string _recordDefinitionFieldIdentifier;

        private string _recordSeparator = null;
        private string _fieldSeparator = null;
        private string _quotingSeparator;
        private readonly AddmlFlatFileFormat _format = AddmlFlatFileFormat.Fixed;

        private Checksum _checksum = null;

        public AddmlFlatFileDefinition Build()
        {
            return new AddmlFlatFileDefinition(_name, _fileName, _fileInfo, _recordSeparator, _fieldSeparator, _quotingSeparator, _charset,
                _recordDefinitionFieldIdentifier, _numberOfRecords, _checksum, _format, _processes);
        }

        public AddmlFlatFileDefinitionBuilder WithName(string name)
        {
            _name = name;
            return this;
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

        public AddmlFlatFileDefinitionBuilder WithRecordSeparator(string recordSeparator)
        {
            _recordSeparator = recordSeparator;
            return this;
        }

        public AddmlFlatFileDefinitionBuilder WithFieldSeparator(string fieldSeparator)
        {
            _fieldSeparator = fieldSeparator;
            return this;

        }

        public AddmlFlatFileDefinitionBuilder WithQuotingSeparator(string quotingSeparator)
        {
            _quotingSeparator = quotingSeparator;
            return this;
        }

        public AddmlFlatFileDefinitionBuilder WithChecksum(Checksum checksum)
        {
            _checksum = checksum;
            return this;
        }

        public AddmlFlatFileDefinitionBuilder WithFileInfo(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            return this;
        }


        public AddmlFlatFileDefinitionBuilder WithRecordDefinitionFieldIdentifier(string fieldIdentifier)
        {
            _recordDefinitionFieldIdentifier = fieldIdentifier;
            return this;
        }
    }
}