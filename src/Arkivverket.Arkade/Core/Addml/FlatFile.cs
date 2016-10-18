using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFile
    {
        public AddmlFlatFileDefinition Definition { private set; get; }

        public FlatFile(AddmlFlatFileDefinition definition)
        {
            Definition = definition;
        }

        public List<string> GetFileProcesses()
        {
            return new List<string>();
        }

        public List<string> GetRecordProcesses()
        {
            return new List<string>();
        }
    }
}