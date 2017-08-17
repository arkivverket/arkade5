using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Test.Core.Addml.Builders
{
    public class AddmlRecordDefinitionBuilder
    {
        private static readonly Random Random = new Random();
        private readonly List<string> _processes = new List<string>();
        private readonly string _recordDefinitionFieldValue = null;

        private AddmlFlatFileDefinition _addmlFlatFileDefinition;
        private string _name = "Record" + Random.Next();
        private int? _recordLength = 100;
        private List<AddmlForeignKey> _foreignKeys = new List<AddmlForeignKey>();

        public AddmlRecordDefinitionBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public AddmlRecordDefinitionBuilder WithAddmlFlatFileDefinition(AddmlFlatFileDefinition addmlFlatFileDefinition)
        {
            _addmlFlatFileDefinition = addmlFlatFileDefinition;
            return this;
        }

        public AddmlRecordDefinitionBuilder WithRecordLength(int recordLength)
        {
            _recordLength = recordLength;
            return this;
        }

        public AddmlRecordDefinition Build()
        {
            if (_addmlFlatFileDefinition == null)
            {
                _addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            }

            var addmlRecordDefinition = new AddmlRecordDefinition(_addmlFlatFileDefinition, _name, _recordLength,
                _recordDefinitionFieldValue, _foreignKeys, _processes);

            _addmlFlatFileDefinition.AddmlRecordDefinitions.Add(addmlRecordDefinition);

            return addmlRecordDefinition;
        }

        public AddmlRecordDefinitionBuilder WithRecordProcess(string processName)
        {
            _processes.Add(processName);
            return this;
        }

        public AddmlRecordDefinitionBuilder WithForeignKey(AddmlForeignKey foreignKey)
        {
            _foreignKeys.Add(foreignKey);
            return this;
        }
    }
}