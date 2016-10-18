using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class AddmlDefinition
    {

        public List<AddmlFlatFileDefinition> AddmlFlatFileDefinitions { get; }

        // TODO: Remove this!
        public AddmlDefinition()
        {
            
        }

        public AddmlDefinition(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions)
        {
            AddmlFlatFileDefinitions = addmlFlatFileDefinitions;
        }

        public List<FlatFile> GetFlatFiles()
        {
            return AddmlFlatFileDefinitions.Select(
                addmlFlatFileDefinition => new FlatFile(addmlFlatFileDefinition)).ToList();
        }

        public List<string> GetFileProcesses()
        {
            return new List<string>();
        }
    }
}