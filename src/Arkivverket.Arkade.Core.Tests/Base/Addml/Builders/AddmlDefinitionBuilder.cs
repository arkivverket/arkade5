using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Builders
{
    public class AddmlDefinitionBuilder
    {
        private List<AddmlFlatFileDefinition> _addmlFlatFileDefinitions;
        private List<AddmlFlatFileDefinition> _addmlFlatFileDefinitionsExistingInDirectory;

        public AddmlDefinitionBuilder()
        {
        }

        public AddmlDefinitionBuilder WithAddmlFlatFileDefinitions(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions, List<AddmlFlatFileDefinition> addmlFlatFileDefinitionsExistingInDirectory)
        {
            _addmlFlatFileDefinitions = addmlFlatFileDefinitions;
            _addmlFlatFileDefinitionsExistingInDirectory = addmlFlatFileDefinitionsExistingInDirectory;
            return this;
        }

        public AddmlDefinition Build()
        {
            return new AddmlDefinition(_addmlFlatFileDefinitions, _addmlFlatFileDefinitionsExistingInDirectory);
        }

    }
}
