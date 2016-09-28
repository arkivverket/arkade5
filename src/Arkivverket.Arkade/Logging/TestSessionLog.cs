using Arkivverket.Arkade.ExternalModels.TestSessionLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Logging
{
    public class TestSessionLog
    {

        private testSessionLog _testSessionLog = new testSessionLog();

        public void SetStartDate(DateTime startDate)
        {
            // TODO
        }

        public void SetEndDate(DateTime startDate)
        {
            // TODO
        }

        public void AddTestResult()
        {
            // TODO
        }

        public void Log(string message)
        {
            var newLogEntry = new logEntriesLogEntry();
            newLogEntry.timestamp = new DateTime();
            newLogEntry.message = message;

            if (_testSessionLog.logEntries == null)
            {
                _testSessionLog.logEntries = new logEntriesLogEntry[0];
            }

            var logEntries = new List<logEntriesLogEntry>(_testSessionLog.logEntries);
            logEntries.Add(newLogEntry);

            _testSessionLog.logEntries = logEntries.ToArray();
        }

        public string CreateXml()
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xml = new XmlSerializer(typeof(testSessionLog));
            xml.Serialize(sw, _testSessionLog);
            return sw.ToString();
        }

    }
}
