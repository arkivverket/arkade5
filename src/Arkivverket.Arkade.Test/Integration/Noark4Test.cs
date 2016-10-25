using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Xunit;

namespace Arkivverket.Arkade.Test.Integration
{
    public class Noark4Test
    {
        [Fact(Skip = "Error in XSLT transformation. arkivuttrekk.xml is not valid")]
        public void ShouldParseNoark4ConvertedArkivuttrekkXml()
        {
            //string noarkihString = TestUtil.ReadFromFileInTestDataDir("noark4\\NOARKIH.XML");
            //string addmlString = NoarkihToAddmlTransformer.Transform(noarkihString);

            // File is converted from NOARKIH.XML format
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark4\\arkivuttrekk.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();

/*
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(10);
            {
                addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
                addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
                addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
                addmlFlatFileDefinitions[0].RecordSeparator.Should().Be(RecordSeparator.CRLF);
                addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(36);
                addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>() {addmlFieldDefinitions[0]});
            }
            */
        }
    }
}
