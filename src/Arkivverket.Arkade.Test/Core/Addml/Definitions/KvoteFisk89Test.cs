using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class KvoteFisk89Test
    {
        [Fact(Skip = "kvotekontroll89-Version8.2.XML does not validate against addml.xsd")]
        public void ShouldParseKvoteFisk89Xml()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\KvoteFisk89\\kvotekontroll89-Version8.2.XML");
            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
        }
    }
}