using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Test.Core.Addml.Builders
{
    public class AddmlRecordDefinitionBuilder
    {

        public AddmlRecordDefinition Build(AddmlFlatFileDefinition addmlFlatFileDefinition, string name)
        {
            return new AddmlRecordDefinition(addmlFlatFileDefinition, name, 100, null, new List<string>());
        }

    }
}
