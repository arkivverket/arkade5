using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class Field : HasProcesses
    {
        public AddmlFieldDefinition Definition { get; }
        public string Value { get; }

        public Field(AddmlFieldDefinition definition, string value)
        {
            Definition = definition;
            Value = GetValue(value);
        }

        public string GetName()
        {
            return Definition.Name;
        }

        public List<string> GetProcesses()
        {
            return Definition.Processes;
        }

        public bool IsPartOfForeignKey(AddmlForeignKey foreignKey)
        {
            return foreignKey.ForeignKeys.Contains(Definition);
        }

        private string GetValue(string value)
        {
            if (Definition is not {AddmlRecordDefinition: {AddmlFlatFileDefinition: { QuotingChar: { }}}})
                return value;

            string quotingChar = Definition.AddmlRecordDefinition.AddmlFlatFileDefinition.QuotingChar;

            if (value.StartsWith(quotingChar) && value.EndsWith(quotingChar))
            {
                value = value.Remove(0, quotingChar.Length);
                value = value.Remove(value.Length - quotingChar.Length);
            }

            return value;
        }
    }
}