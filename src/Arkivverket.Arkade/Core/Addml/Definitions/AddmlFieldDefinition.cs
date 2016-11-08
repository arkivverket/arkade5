using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFieldDefinition
    {
        public AddmlRecordDefinition AddmlRecordDefinition { get; }

        public string Name { get; }
        public int? StartPosition { get; }
        public int? FixedLength { get; }

        // TODO: Implemenr DataType with fieldFormat, alignment, etc
        public DataType Type { get; }
        public bool IsUnique { get; }
        public bool IsNullable { get; }
        public int? MinLength { get; }
        public int? MaxLength { get; }
        //public string Codes { get; }
        public AddmlFieldDefinition ForeignKey { get; }
        public List<string> Processes { get; }

        public AddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            DataType type,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            AddmlFieldDefinition foreignKey,
            AddmlRecordDefinition addmlRecordDefinition,
            List<string> processes)
        {
            Name = name;
            StartPosition = startPosition;
            FixedLength = fixedLength;
            Type = type;
            IsUnique = isUnique;
            IsNullable = isNullable;
            MinLength = minLength;
            MaxLength = maxLength;
            ForeignKey = foreignKey;
            AddmlRecordDefinition = addmlRecordDefinition;
            Processes = processes;
        }

        public AddmlFlatFileDefinition GetAddmlFlatFileDefinition()
        {
            return AddmlRecordDefinition.AddmlFlatFileDefinition;
        }

        public bool IsPartOfPrimaryKey()
        {
            return AddmlRecordDefinition.PrimaryKey.Contains(this);
        }

        public string Key()
        {
            return AddmlRecordDefinition.Key() + "_" + Name;
        }

        public FieldIndex GetFieldIndeks()
        {
            return new FieldIndex(AddmlRecordDefinition.AddmlFlatFileDefinition.Name, AddmlRecordDefinition.Name, this.Name);
        }
    }
}
