using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.TestSessionLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Logging
{
    public class TestSessionXmlGenerator
    {

        public static string GenerateXml(TestSession testSession)
        {
            testSessionLog log = new testSessionLog();
            log.timestamp = new DateTime();
            log.arkadeVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

            log.archiveType = testSession?.Archive?.ArchiveType.ToString();
            log.archiveUuid = testSession?.Archive?.Uuid;

            log.logEntries = GetLogEntries(testSession);
            log.testResults = GetTestResults(testSession);

            return CreateXml(log);
        }

        private static testResultsTestResult[] GetTestResults(TestSession testSession)
        {
            var xmlTestResulta = new List<testResultsTestResult>();
            /* TODO jostein
            foreach (LogEntry logEntry in testSession.Get)
            {
                var xmlLogEntry = new logEntriesLogEntry();
                xmlLogEntry.timestamp = logEntry.Timestamp;
                xmlLogEntry.message = logEntry.Message;
                xmlTestResulta.Add(xmlLogEntry);
            }
            */
            return xmlTestResulta.ToArray();
        }

        private static logEntriesLogEntry[] GetLogEntries(TestSession testSession)
        {
            var xmlLogEntries = new List<logEntriesLogEntry>();
            foreach (LogEntry logEntry in testSession.GetLogEntries())
            {
                var xmlLogEntry = new logEntriesLogEntry();
                xmlLogEntry.timestamp = logEntry.Timestamp;
                xmlLogEntry.message = logEntry.Message;
                xmlLogEntries.Add(xmlLogEntry);
            }
            return xmlLogEntries.ToArray();
        }

        private static string CreateXml(testSessionLog log)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xml = new XmlSerializer(typeof(testSessionLog));
            xml.Serialize(sw, log);
            return sw.ToString();
        }

    }
}
