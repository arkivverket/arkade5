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
        public void XmlShouldContainBasicData()
        {
            TestSession testSession = new TestSessionBuilder()
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            new TestSessionLogXmlAssert(xml)
                .assertTimestampNow()
                .assertArchiveUuid(testSession.Archive.Uuid)
                .assertArchiveType(testSession.Archive.ArchiveType)
                .assertArkadeVersion("unknown");
        }

        [Fact]
        public void XmlShouldContainLogEntries()
        {
            TestSession testSession = new TestSessionBuilder()
                .WithLogEntry("Log line 1")
                .WithLogEntry("Log line 2")
                .WithLogEntry("Log line 3")
                .WithLogEntry("Log line 4")
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            new TestSessionLogXmlAssert(xml)
                .assertLogEntryMessage("Log line 1")
                .assertLogEntryMessage("Log line 2")
                .assertLogEntryMessage("Log line 3")
                .assertLogEntryMessage("Log line 4");
        }

        [Fact(Skip = "Work in progress!")]
        public void XmlShouldContainTestResults()
        {
            TestSession testSession = new TestSessionBuilder()
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            TestSessionLogXmlValidator.Validate(xml);
            xml.Should()
                .Contain("<testName>testName</testName>")
                .And
                .Contain("<testCategory>testCategory</testCategory>")
                .And
                .Contain("<durationMillis>1234</durationMillis>")
                .And
                .Contain("<status>SUCCESS</status>")
                .And
                .Contain("<message>No message</message>");
        }

    }
}
