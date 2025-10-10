using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.TestSessionLog;
using Arkivverket.Arkade.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Testing;
using System.Text;
using Serilog;

namespace Arkivverket.Arkade.Core.Logging
{
    public class TestSessionXmlGenerator
    {
        private static ILogger _log = Log.ForContext<TestSessionXmlGenerator>();

        public void GenerateXmlAndSaveToFile(Archive archive)
        {
            string pathToLogFile = new ArkadeDirectory(archive.TestSession.TemporaryTestResultFilesDirectory)
                .WithFile(ArkadeConstants.ArkadeXmlLogFileName)
                .FullName;

            testSessionLog log = GetTestSessionLog(archive);
            FileStream fs = new FileStream(pathToLogFile, FileMode.Create);

            XmlSerializer xmls = new XmlSerializer(typeof(testSessionLog));
            xmls.Serialize(fs, log);
            fs.Close();
        }

        public static string GenerateXml(Archive archive)
        {
            return CreateXml(GetTestSessionLog(archive));
        }

        private static testSessionLog GetTestSessionLog(Archive archive)
        {
            testSessionLog log = new testSessionLog();
            log.timestamp = DateTime.Now;
            log.arkadeVersion = ArkadeVersion.Current;

            log.archiveType = archive.ArchiveType.ToString();
           // log.archiveUuid = testSession?.Archive?.OriginalUuid?.GetValue(); // NB! UUID-writeout (test results)

            log.logEntries = GetLogEntries(archive.TestSession);
            log.testResults = GetTestResults(archive.TestSession);

            return log;
        }

        private static testResultsTestResult[] GetTestResults(TestSession testSession)
        {
            var xmlTestResults = new List<testResultsTestResult>();
            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                var testResult = new testResultsTestResult();
                testResult.testName = testRun.TestName;
                testResult.testCategory = testRun.TestType.ToString();
                testResult.durationMillis = testRun.TestDuration.ToString();
                testResult.testDescription = testRun.TestDescription;
                testResult.status = testRun.IsSuccess() ? "SUCCESS" : "ERROR";
                testResult.message = ConcatMessages(testRun.TestResults.GetAllResults());
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
