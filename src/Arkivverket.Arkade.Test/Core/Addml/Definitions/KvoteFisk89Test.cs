using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class KvoteFisk89Test
    {
        [Fact(Skip = "Currently unable to parse this")]
        public void ShouldParseKvoteFisk89Xml()
        {
            addml addml = AddmlUtil.ReadFromFile(
                  $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\TestData\\KvoteFisk89\\kvotekontroll89-Version8.2.XML");
            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
        }
    }
}