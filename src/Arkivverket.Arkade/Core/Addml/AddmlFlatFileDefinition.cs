using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlFlatFileDefinition
    {
        public string Name { get; }
        public string FileName { get; }
        public int RecordLength { get; }
        public List<AddmlFieldDefinition> AddmlFieldDefinitions { get; }
        public string RecordSeparator { get; }
        public string FieldSeparator { get; }
        public string Charset { get; }
        public int NumberOfOccurences { get; }
        public string ChecksumAlgorithm { get; }
        public string ChecksumValue { get; }

        // FileProcesses
        // RecordProcesses

        public AddmlFlatFileDefinition(string name, string fileName, int recordLength)
        {
            Name = name;
            FileName = fileName;
            RecordLength = recordLength;

        }


    }
}
