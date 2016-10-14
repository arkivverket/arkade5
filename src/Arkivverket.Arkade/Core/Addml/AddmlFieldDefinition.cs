using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlFieldDefinition
    {
        public AddmlFlatFileDefinition AddmlFlatFileDefinition { get; }

        public string Name { get; }
        public string StartPosition { get; }
        public string FixedLength { get; }
        public bool IsPrimaryKey { get; }
        public string Type { get; }
        public AddmlFieldDefinition ForeignKey { get; }
        public bool IsUnique { get; }
        public bool IsNullable { get; }
        public int MinLength { get; }
        public int MaxLength { get; }
        public string codes { get; }

    }
}
