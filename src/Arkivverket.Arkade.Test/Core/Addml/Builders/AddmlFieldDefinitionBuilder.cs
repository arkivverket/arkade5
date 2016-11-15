using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Test.Core.Addml.Builders
{
    public class AddmlFieldDefinitionBuilder
    {
        private string _name = "Field" + new Random().Next();
        private int? _startPosition = null;
        private int? _fixedLength = null;
        private DataType _dataType = StringDataType.Default;
        private bool _isUnique = false;
        private bool _isNullable = true;
        private int? _minLength = null;
        private int? _maxLength = null;
        private AddmlFieldDefinition _foreignKey = null;
        private AddmlRecordDefinition _addmlRecordDefinition;
        private List<string> _processes = new List<string>();

        public AddmlFieldDefinition Build()
        {

            if (_addmlRecordDefinition == null)
            {
                _addmlRecordDefinition = new AddmlRecordDefinitionBuilder().Build();
            }

            AddmlFieldDefinition addmlFieldDefinition = new AddmlFieldDefinition(
                _name,
                _startPosition,
                _fixedLength,
                _dataType,
                _isUnique,
                _isNullable,
                _minLength,
                _maxLength,
                _foreignKey,
                _addmlRecordDefinition,
                _processes);

            return addmlFieldDefinition;
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
    }
}
