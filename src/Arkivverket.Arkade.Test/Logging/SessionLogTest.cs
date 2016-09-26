using Arkivverket.Arkade.Logging;
using System;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Logging
{
    public class SessionLogTest
    {
        [Fact]
        public void ShouldGenerateXml()
        {
            SessionLog SesionLog = new SessionLog();
            SesionLog.LogInfo("Logging on INFO");

            string xml = SesionLog.CreateXml();
            xml.Should().Contain("<logLevel>INFO</logLevel>")
                .And.Contain("<message>Logging on INFO</message>");
        }
    }
}
