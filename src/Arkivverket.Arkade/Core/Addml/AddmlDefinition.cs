using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
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
            List<FlatFile> flatFiles = new List<FlatFile>();
            foreach (AddmlFlatFileDefinition addmlFlatFileDefinition in AddmlFlatFileDefinitions)
            {
                flatFiles.Add(new FlatFile(addmlFlatFileDefinition));
            }
            return flatFiles;
        }

        public List<string> GetFileProcesses()
        {
            return new List<string>();
        }
    }
}