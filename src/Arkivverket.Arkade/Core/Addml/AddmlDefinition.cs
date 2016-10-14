using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDefinition
    {

        public List<Field> Fields { private set; get; }

        private List<AddmlFlatFileDefinition> AddmlFlatFileDefinitions;

        public List<FlatFile> GetFlatFiles()
        {
            return new List<FlatFile>();
        }

        public List<string> GetFileProcesses()
        {
            return new List<string>();
        }
    }
}