using Arkivverket.Arkade.Core.Addml.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Test.Core.Addml.Builders
{
    public class AddmlDefinitionBuilder
    {
        private List<AddmlFlatFileDefinition> _addmlFlatFileDefinitions;

        public AddmlDefinitionBuilder()
        {
        }

        public AddmlDefinitionBuilder WithAddmlFlatFileDefinitions(List<AddmlFlatFileDefinition> addmlFlatFileDefinitions)
        {
            _addmlFlatFileDefinitions = addmlFlatFileDefinitions;
            return this;
        }

        public AddmlDefinition Build()
        {
            return new AddmlDefinition(_addmlFlatFileDefinitions);
        }

    }
}
