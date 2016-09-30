using Arkivverket.Arkade.Logging;
using Xunit;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests;

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

        [Fact]
        public void XmlShouldContainTestResults()
        {
            TestSession testSession = new TestSessionBuilder()
                .WithTestRun(new TestRunBuilder()
                    .WithTestName("test1")
                    .WithTestCategory("category1")
                    .WithDurationMillis(123)
                    .WithStatus(ResultType.Success)
                    .WithMessage("message1")
                    .Build())
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            new TestSessionLogXmlAssert(xml)
                .AssertNumberOfTestResult(1)
                .FirstTestResult()
                .AssertTestName("test1")
                .AssertTestCategory("category1")
                .AssertDurationMillis(123)
                .AssertStatus("SUCCESS")
                .AssertMessage("message1")
                ;
        }

    }
}
