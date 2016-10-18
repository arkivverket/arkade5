using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFile
    {
        public AddmlFlatFileDefinition Definition { get; }

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
            return Definition.AddmlRecordDefinitions.SelectMany(d => d.Processes).ToList();
        }
    }
}