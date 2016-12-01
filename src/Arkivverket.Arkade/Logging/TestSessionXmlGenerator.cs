using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.TestSessionLog;
using Arkivverket.Arkade.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Arkivverket.Arkade.Tests;
using System.Text;
using Serilog;

namespace Arkivverket.Arkade.Logging
{
    public class TestSessionXmlGenerator
    {
        private static ILogger _log = Log.ForContext<TestSessionXmlGenerator>();

        public static void GenerateXmlAndSaveToFile(TestSession testSession)
        {
            DirectoryInfo workingDirectory = testSession.Archive.WorkingDirectory;
            string pathToLogFile = workingDirectory.FullName + "\\arkade-log.xml";
            _log.Information("Writing xml log file to {LogFile}", pathToLogFile);

            testSessionLog log = GetTestSessionLog(testSession);
            FileStream fs = new FileStream(pathToLogFile, FileMode.Create);

            XmlSerializer xmls = new XmlSerializer(typeof(testSessionLog));
            xmls.Serialize(fs, log);
            fs.Close();
        }

        public static string GenerateXml(TestSession testSession)
        {
            return CreateXml(GetTestSessionLog(testSession));
        }

        private static testSessionLog GetTestSessionLog(TestSession testSession)
        {
            testSessionLog log = new testSessionLog();
            log.timestamp = DateTime.Now;
            log.arkadeVersion = ArkadeVersion.Version;

            log.archiveType = testSession?.Archive?.ArchiveType.ToString();
            log.archiveUuid = testSession?.Archive?.Uuid?.GetValue();

            log.logEntries = GetLogEntries(testSession);
            log.testResults = GetTestResults(testSession);

            return log;
        }

        private static testResultsTestResult[] GetTestResults(TestSession testSession)
        {
            var xmlTestResults = new List<testResultsTestResult>();
            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                var testResult = new testResultsTestResult();
                testResult.testName = testRun.TestName;
                testResult.testCategory = testRun.TestCategory;
                testResult.durationMillis = testRun.TestDuration.ToString();
                testResult.testDescription = testRun.TestDescription;
                testResult.status = testRun.IsSuccess() ? "SUCCESS" : "ERROR";
                testResult.message = ConcatMessages(testRun.Results);
                xmlTestResults.Add(testResult);
            }
            return xmlTestResults.Count == 0 ? null : xmlTestResults.ToArray();
        }

        private static string ConcatMessages(List<TestResult> results)
        {
            StringBuilder sb = new StringBuilder("");
            foreach (var result in results.Take(100)) // TODO only first 100 elements included due to out of memory issue
            {
                if (!string.IsNullOrWhiteSpace(result.Location.ToString()))
                    sb.Append("[").Append(result.Location).Append("] ");

                sb.AppendLine(result.Message);
            }

            return sb.ToString();
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
            return xmlLogEntries.Count == 0 ? null : xmlLogEntries.ToArray();
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
