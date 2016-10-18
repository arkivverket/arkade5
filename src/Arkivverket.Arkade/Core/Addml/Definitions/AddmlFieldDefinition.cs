using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlFieldDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public string Name { get; }
        public int? StartPosition { get; }
        public int? FixedLength { get; }
        public bool IsPrimaryKey { get; }
        public string Type { get; }
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
            string type,
            bool isPrimaryKey,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            AddmlFieldDefinition foreignKey,
            AddmlFlatFileDefinition addmlFlatFileDefinition,
            List<string> processes)
        {
            Name = name;
            StartPosition = startPosition;
            FixedLength = fixedLength;
            Type = type;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
            IsNullable = isNullable;
            MinLength = minLength;
            MaxLength = maxLength;
            ForeignKey = foreignKey;
            AddmlFlatFileDefinition = addmlFlatFileDefinition;
            Processes = processes;
        }

    }
}
