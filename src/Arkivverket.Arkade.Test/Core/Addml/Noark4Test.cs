using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class Noark4Test
    {
        [Fact]
        public void ShouldParseNoark4ConvertedArkivuttrekkXml()
        {
            // Code to convert NOARKIH.XML to addml.xml
            //string noarkihString = TestUtil.ReadFromFileInTestDataDir("noark4/NOARKIH.XML");
            //string addmlString = NoarkihToAddmlTransformer.Transform(noarkihString);

            // File is converted from NOARKIH.XML format

            var externalContentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/TestData/noark4");
            var workingDirectory = new WorkingDirectory(ArkadeProcessingArea.WorkDirectory, externalContentDirectory);
            AddmlInfo addml = AddmlUtil.ReadFromFile(workingDirectory.Content().WithFile("addml.xml").FullName);

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml, workingDirectory, new StatusEventHandler());

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();

            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(63);
            addmlFlatFileDefinitions[0].Name.Should().Be("ORDNPRINSTYPE");
            addmlFlatFileDefinitions[0].FileName.Should().Be("OPRITYP.XML");
            addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
            addmlFlatFileDefinitions[0].RecordSeparator.Should().BeNull();
            addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
            AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
            addmlRecordDefinition.Name.Should().Be("ORDNPRINSTYPE");
            List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
            addmlFieldDefinitions.Count.Should().Be(2);
            addmlFieldDefinitions[0].Name.Should().Be("OT.KODE");
            addmlFieldDefinitions[0].Type.Should().Be(StringDataType.Default);
            addmlFieldDefinitions[1].Name.Should().Be("OT.BETEGN");
            addmlFieldDefinitions[1].Type.Should().Be(StringDataType.Default);
            addmlRecordDefinition.PrimaryKey.Should().BeNull();
        }
    }
}