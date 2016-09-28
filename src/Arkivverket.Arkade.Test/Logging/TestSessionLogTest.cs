using Arkivverket.Arkade.Logging;
using System;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Logging
{
    public class TestSessionLogTest
    {
        [Fact]
        public void ShouldGenerateXml()
        {
            TestSessionLog SesionLog = new TestSessionLog();
            SesionLog.Log("Logging!");

            string xml = SesionLog.CreateXml();
            xml.Should().Contain("<timestamp>")
                .And.Contain("<message>Logging!</message>");
        }
    }
}
