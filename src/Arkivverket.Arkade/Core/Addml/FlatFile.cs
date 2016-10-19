using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFile : HasProcesses
    {
        public AddmlFlatFileDefinition Definition { get; }

        public FlatFile(AddmlFlatFileDefinition definition)
        {
            Definition = definition;
        }

        public string GetName()
        {
            return Definition.Name;
        }

        public List<string> GetProcesses()
        {
            return Definition.Processes;
        }

        public List<string> GetRecordProcesses()
        {
            return Definition.AddmlRecordDefinitions.SelectMany(d => d.Processes).ToList();
        }

    }
}