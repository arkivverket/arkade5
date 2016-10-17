using System;
using Xunit;
using Arkivverket.Arkade.Core.Addml;
using System.IO;
using Arkivverket.Arkade.Util;
using Arkivverket.Arkade.ExternalModels.Addml;
using System.Collections.Generic;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class AddmlDefinitionParserTest
    {
        [Fact]
        public void ShouldParseJegerreg98ArkivuttrekkXml()
        {
            string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\jegerreg-98-dos\\arkivuttrekk.xml";
            string fileContent = File.ReadAllText(fileName);
            addml addml = SerializeUtil.DeserializeFromString<addml>(fileContent);

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            List<AddmlFlatFileDefinition> addmlFlatFileDefinition = parser.GetAddmlFlatFileDefinitions();
            addmlFlatFileDefinition.Count.Should().Be(10);

        }
    }
}
