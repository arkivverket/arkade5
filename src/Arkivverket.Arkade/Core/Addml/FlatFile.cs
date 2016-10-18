using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

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
            return Definition.Processes;
        }

        public List<string> GetRecordProcesses()
        {
            return new List<string>();
        }
    }
}