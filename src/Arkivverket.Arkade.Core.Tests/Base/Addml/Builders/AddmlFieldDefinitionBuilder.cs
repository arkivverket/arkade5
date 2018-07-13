using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Builders
{
    public class AddmlFieldDefinitionBuilder
    {
        private static readonly Random Random = new Random();

        private string _name = "Field" + Random.Next();
        private readonly int? _startPosition = null;
        private int? _fixedLength;
        private DataType _dataType = StringDataType.Default;
        private bool _isUnique = false;
        private bool _isNullable = true;
        private int? _minLength;
        private int? _maxLength;
        private AddmlRecordDefinition _addmlRecordDefinition;
        private readonly List<string> _processes = new List<string>();
        private List<AddmlCode> _codes;
        private bool _isPartOfPrimaryKey;

        public AddmlFieldDefinition Build()
        {

            if (_addmlRecordDefinition == null)
            {
                _addmlRecordDefinition = new AddmlRecordDefinitionBuilder().Build();
            }

            var addmlFieldDefinition = _addmlRecordDefinition.AddAddmlFieldDefinition(_name,
                _startPosition,
                _fixedLength,
                _dataType,
                _isUnique,
                _isNullable,
                _minLength,
                _maxLength,
                _processes,
                _codes,
                _isPartOfPrimaryKey);

            return addmlFieldDefinition;
        }

        internal AddmlFieldDefinitionBuilder WithFixedLength(int fixedLength)
        {
            _fixedLength = fixedLength;
            return this;
        }

        public AddmlFieldDefinitionBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public AddmlFieldDefinitionBuilder WithRecordDefinition(AddmlRecordDefinition recordDefinition)
        {
            _addmlRecordDefinition = recordDefinition;
            return this;
        }

        public AddmlFieldDefinitionBuilder WithDataType(DataType dataType)
        {
            _dataType = dataType;
            return this;
        }

        public AddmlFieldDefinitionBuilder WithCodeList(List<AddmlCode> codes)
        {
            _codes = codes;
            return this;
        }
        public AddmlFieldDefinitionBuilder WithMinLength(int minLength)
        {
            _minLength = minLength;
            return this;
        }
        public AddmlFieldDefinitionBuilder WithMaxLength(int maxLength)
        {
            _maxLength = maxLength;
            return this;
        }
        public AddmlFieldDefinitionBuilder IsPartOfPrimaryKey(bool isPartOfPrimaryKey)
        {
            _isPartOfPrimaryKey = isPartOfPrimaryKey;
            return this;
        }

    }
}
