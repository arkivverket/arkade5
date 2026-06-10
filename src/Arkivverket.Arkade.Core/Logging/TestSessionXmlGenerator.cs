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
using Arkivverket.Arkade.Core.Base.Archives;
using Serilog;

namespace Arkivverket.Arkade.Core.Logging
{
    public class TestSessionXmlGenerator
    {
        private static ILogger _log = Log.ForContext<TestSessionXmlGenerator>();

        public void GenerateXmlAndSaveToFile(Archive archive, OutputDiasPackage outputDiasPackage)
        {
            // The test-session log ships inside the output package's repository_operations (AIP only —
            // SIP omits that directory). It is written here, at package creation, so it can carry the
            // output package's UUID rather than any input identity.
            string pathToLogFile = outputDiasPackage.WorkingDirectory.RepositoryOperations()
                .WithFile(ArkadeConstants.ArkadeXmlLogFileName)
                .FullName;

            testSessionLog log = GetTestSessionLog(archive, outputDiasPackage.Id);
            FileStream fs = new FileStream(pathToLogFile, FileMode.Create);

            XmlSerializer xmls = new XmlSerializer(typeof(testSessionLog));
            xmls.Serialize(fs, log);
            fs.Close();
        }

        public static string GenerateXml(Archive archive)
        {
            return CreateXml(GetTestSessionLog(archive, archive.OutputDiasPackage?.Id));
        }

        private static testSessionLog GetTestSessionLog(Archive archive, Uuid packageUuid)
        {
            testSessionLog log = new testSessionLog();
            log.timestamp = DateTime.Now;
            log.arkadeVersion = ArkadeVersion.Current;

            log.archiveType = archive.ArchiveType.ToString();
            // archiveUuid is required by testSessionLog.xsd (minOccurs=1), so the element must always be present.
            // The log carries the output package's UUID; coalesce to "-" defensively (matching the test
            // report's placeholder), though a package being created always has an Id.
            log.archiveUuid = packageUuid?.GetValue() ?? "-"; // NB! UUID-writeout (package creation)

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
