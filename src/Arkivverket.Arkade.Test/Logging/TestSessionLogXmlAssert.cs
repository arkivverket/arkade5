using Arkivverket.Arkade.ExternalModels.TestSessionLog;
using System;
using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Logging
{
    public class TestSessionLogXmlAssert
    {

        private readonly testSessionLog _testSessionLog;
        private readonly string _xml;

        public TestSessionLogXmlAssert(string xml)
        {
            _xml = xml;
            _testSessionLog = DeserializeXml(xml);

            TestSessionLogXmlValidator.Validate(xml);
        }

        private testSessionLog DeserializeXml(string xmlString)
        {
            StringReader sr = new StringReader(xmlString);
            XmlSerializer xml = new XmlSerializer(typeof(testSessionLog));
            return (testSessionLog) xml.Deserialize(sr);
        }

        public TestSessionLogXmlAssert AssertTimestampNow()
        {
            DateTime now = DateTime.Now;
            DateTime tenSecondsAgo = now.Add(new TimeSpan(0, 0, -10));
            DateTime tenSecondsFromNow = now.Add(new TimeSpan(0, 0, 10));

            DateTime timestampFromXml = ((DateTime)_testSessionLog.timestamp);

            if (timestampFromXml < tenSecondsAgo || timestampFromXml > tenSecondsFromNow)
            {
                throw new SystemException("Timestamp in XML is not now! " + timestampFromXml);
            }

            return this;
        }

        public TestSessionLogXmlAssert AssertArchiveUuid(Uuid archiveUuid)
        {
            _testSessionLog.archiveUuid.Should().Be(archiveUuid.GetValue());
            return this;
        }

        public TestSessionLogXmlAssert AssertArchiveType(ArchiveType archiveType)
        {
            _testSessionLog.archiveType.Should().Be(archiveType.ToString());
            return this;
        }

        public TestSessionLogXmlAssert AssertArkadeVersion(string arkadeVersion)
        {
            _testSessionLog.arkadeVersion.Should().Be(arkadeVersion);
            return this;
        }

        public TestSessionLogXmlAssert AssertLogEntryMessage(string message)
        {
            bool found = false;

            foreach (var logEntry in _testSessionLog.logEntries)
            {
                if (logEntry.message.Equals(message))
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new SystemException("Unable to find message ["+message+"] in logEntries. XML [" + _xml + "]");
            }

            return this;
        }

        public TestSessionLogXmlAssert AssertNumberOfTestResult(int numberOfTestResults)
        {
            _testSessionLog.testResults.Length.Should().Be(numberOfTestResults);
            return this;
        }

        public TestSessionLogTestResultXmlAssert FirstTestResult()
        {
            testResultsTestResult first = _testSessionLog.testResults[0];
            return new TestSessionLogTestResultXmlAssert(first);
        }

    }

    public class TestSessionLogTestResultXmlAssert
    {

        private testResultsTestResult _testResultsTestResult;

        public TestSessionLogTestResultXmlAssert(testResultsTestResult testResultsTestResult)
        {
            _testResultsTestResult = testResultsTestResult;
        }

        public TestSessionLogTestResultXmlAssert AssertTestName(string testName)
        {
            _testResultsTestResult.testName.Should().Be(testName);
            return this;
        }
    }
}
