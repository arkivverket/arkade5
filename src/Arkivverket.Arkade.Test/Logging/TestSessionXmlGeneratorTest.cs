using Arkivverket.Arkade.Logging;
using System;
using Xunit;
using FluentAssertions;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;

namespace Arkivverket.Arkade.Test.Logging
{
    public class TestSessionXmlGeneratorTest
    {
        [Fact]
        public void ShouldGenerateXml()
        {
            TestSession testSession = new TestSessionBuilderForTesting()
                .WithLogEntry("Logging!")
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            xml.Should().Contain("<timestamp>")
                .And.Contain("<message>Logging!</message>");
        }
    }
}
