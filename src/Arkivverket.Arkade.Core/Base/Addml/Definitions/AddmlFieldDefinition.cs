using System;
using System.Collections.Generic;
using System.Reflection;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Serilog;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlFieldDefinition
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public AddmlRecordDefinition AddmlRecordDefinition { get; }

        public string Name { get; }
        //public string Description { get;  }
        public int? StartPosition { get; }
        public int? FixedLength { get; }
        public DataType Type { get; }
        public bool IsUnique { get; }
        public bool IsNullable { get; }
        public int? MinLength { get; }
        public int? MaxLength { get; }
        public List<string> Processes { get; }
        public List<AddmlCode> Codes { get; }
        private readonly FieldIndex _index;

        public AddmlFieldDefinition(string name,
            int? startPosition,
            int? fixedLength,
            DataType type,
            bool isUnique,
            bool isNullable,
            int? minLength,
            int? maxLength,
            AddmlRecordDefinition addmlRecordDefinition,
            List<string> processes,
            List<AddmlCode> codes)
        {
            Name = name;
            StartPosition = startPosition;
            FixedLength = fixedLength;
            Type = type;
            IsUnique = isUnique;
            IsNullable = isNullable;
            MinLength = minLength;
            MaxLength = maxLength;
            AddmlRecordDefinition = addmlRecordDefinition;
            Processes = processes;
            Codes = codes;

            _index = new FieldIndex(AddmlRecordDefinition.AddmlFlatFileDefinition.Name, AddmlRecordDefinition.Name, Name);
        }

        public AddmlFlatFileDefinition GetAddmlFlatFileDefinition()
        {
            return AddmlRecordDefinition.AddmlFlatFileDefinition;
        }

        public bool IsPartOfPrimaryKey()
        {
            return AddmlRecordDefinition.PrimaryKey == null ? false : AddmlRecordDefinition.PrimaryKey.Contains(this);
        }

        [Obsolete("Use GetIndex instead")]
        public string Key()
        {
            return AddmlRecordDefinition.Key() + "_" + Name;
        }

        public FieldIndex GetIndex()
        {
            return _index;
        }

        public bool HasProcess(string processName)
        {
            return Processes.Contains(processName);
        }

        public void AddProcess(string processName)
        {
            if (!HasProcess(processName))
            {
                Processes.Add(processName);
                Log.Debug($"Adding process {processName} to definition {_index}");
            }
            else
            {
                Log.Debug($"Definition {_index} already contains process {processName}");
            }
        }

    }
}
