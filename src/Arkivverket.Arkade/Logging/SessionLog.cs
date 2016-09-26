using Arkivverket.Arkade.ExternalModels.SessionLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Logging
{
    public class SessionLog
    {

        private sessionLog _sessionLog = new sessionLog();

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

        public void LogInfo(string message)
        {
            Log(LogLevel.INFO, message);
        }

        public void LogWarn(string message)
        {
            Log(LogLevel.WARN, message);
        }

        public void LogError(string message)
        {
            Log(LogLevel.ERROR, message);
        }

        private void Log(LogLevel logLevel, string message)
        {
            var newLogEntry = new logEntriesLogEntry();
            newLogEntry.timestamp = new DateTime();
            newLogEntry.logLevel = logLevel.ToString();
            newLogEntry.message = message;

            if (_sessionLog.logEntries == null)
            {
                _sessionLog.logEntries = new logEntriesLogEntry[0];
            }

            var logEntries = new List<logEntriesLogEntry>(_sessionLog.logEntries);
            logEntries.Add(newLogEntry);

            _sessionLog.logEntries = logEntries.ToArray();
        }

        private enum LogLevel
        {
            INFO,
            WARN,
            ERROR
        }

        public string CreateXml()
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xml = new XmlSerializer(typeof(sessionLog));
            xml.Serialize(sw, _sessionLog);
            return sw.ToString();
        }

    }
}
