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
                .AssertTimestampNow()
                .AssertArchiveUuid(testSession.Archive.Uuid)
                .AssertArchiveType(testSession.Archive.ArchiveType)
                .AssertArkadeVersion("unknown");
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
                .AssertLogEntryMessage("Log line 1")
                .AssertLogEntryMessage("Log line 2")
                .AssertLogEntryMessage("Log line 3")
                .AssertLogEntryMessage("Log line 4");
        }

        [Fact(Skip ="Work in progress!")]
        public void XmlShouldContainTestResults()
        {
            TestSession testSession = new TestSessionBuilder()
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            new TestSessionLogXmlAssert(xml)
                .AssertNumberOfTestResult(1)
                .FirstTestResult()
                .AssertTestName("testName")
                //.AssertTestCategory(testCategory)
                //.AssertDurationMillisPresent()
                //.AssertStatus("SUCCESS")
                //.AssertMessage(message)
                ;
        }

    }
}
