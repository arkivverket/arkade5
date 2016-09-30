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

        public TestSessionLogXmlAssert assertTimestampNow()
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

        public TestSessionLogXmlAssert assertArchiveUuid(string archiveUuid)
        {
            _testSessionLog.archiveUuid.Should().Be(archiveUuid);
            return this;
        }

        public TestSessionLogXmlAssert assertArchiveType(ArchiveType archiveType)
        {
            _testSessionLog.archiveType.Should().Be(archiveType.ToString());
            return this;
        }

        public TestSessionLogXmlAssert assertArkadeVersion(string arkadeVersion)
        {
            _testSessionLog.arkadeVersion.Should().Be(arkadeVersion);
            return this;
        }

        public TestSessionLogXmlAssert assertLogEntryMessage(string message)
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



    }
}
