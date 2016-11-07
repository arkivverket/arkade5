using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class Kvotekontroll89Test
    {
        [Fact(Skip = "kvotekontroll89.XML does not validate against addml.xsd")]
        public void ShouldParseKvotekontroll89Xml()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\kvotekontroll89\\kvotekontroll89.XML");
            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
        }
    }
}