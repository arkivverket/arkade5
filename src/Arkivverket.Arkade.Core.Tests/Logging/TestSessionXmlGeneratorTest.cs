using System.IO;
using Arkivverket.Arkade.Core.Logging;
using Xunit;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Tests.Base;

namespace Arkivverket.Arkade.Core.Tests.Logging
{
    public class TestSessionXmlGeneratorTest
    {
        private static Archive CreateArchive()
        {
            return new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark3)
                .WithWorkingDirectoryExternalContent(Path.Combine("TestData", "noark3"))
                .Build();
        }

        [Fact]
        public void XmlShouldContainBasicData()
        {
            Archive archive = CreateArchive();
            archive.OutputDiasPackage = new OutputDiasPackage(PackageType.ArchivalInformationPackage,
                new ArchiveMetadata(), archive.ProcessingDirectory);

            new TestSessionBuilder()
                .WithArchive(archive)
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(archive);

            new TestSessionLogXmlAssert(xml)
                .AssertTimestampNow()
                .AssertArchiveUuid(archive.OutputDiasPackage.Id)
                .AssertArchiveType(archive.ArchiveType)
                .AssertArkadeVersionIsSet();
        }

        [Fact]
        public void XmlShouldContainLogEntries()
        {
            Archive archive = CreateArchive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithLogEntry("Log line 1")
                .WithLogEntry("Log line 2")
                .WithLogEntry("Log line 3")
                .WithLogEntry("Log line 4")
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(archive);

            new TestSessionLogXmlAssert(xml)
                .AssertLogEntryMessage("Log line 1")
                .AssertLogEntryMessage("Log line 2")
                .AssertLogEntryMessage("Log line 3")
                .AssertLogEntryMessage("Log line 4");
        }

        [Fact]
        public void XmlShouldContainTestResults()
        {
            Archive archive = CreateArchive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithTestRun(new TestRunBuilder()
                    .WithTestName("test1")
                    .WithDurationMillis(123)
                    .WithTestResult(new TestResult(ResultType.Success, new Location("location"), "message1"))
                    .Build())
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(archive);

            new TestSessionLogXmlAssert(xml)
                .AssertNumberOfTestResult(1)
                .FirstTestResult()
                .AssertTestName("U.00 - ") // display name is TestId-based, not the mock's name
                .AssertDurationMillis(123)
                .AssertStatus("SUCCESS")
                .AssertMessage("[location] message1")
                ;
        }

        [Fact]
        public void XmlShouldContainTestResultsWithoutLocationWhenEmpty()
        {
            Archive archive = CreateArchive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithTestRun(new TestRunBuilder()
                    .WithTestName("test1")
                    .WithDurationMillis(123)
                    .WithTestResult(new TestResult(ResultType.Success, new Location(""), "message1"))
                    .Build())
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(archive);

            new TestSessionLogXmlAssert(xml)
                .AssertNumberOfTestResult(1)
                .FirstTestResult()
                .AssertTestName("U.00 - ") // display name is TestId-based, not the mock's name
                .AssertDurationMillis(123)
                .AssertStatus("SUCCESS")
                .AssertMessage("message1")
                ;
        }
    }
}
