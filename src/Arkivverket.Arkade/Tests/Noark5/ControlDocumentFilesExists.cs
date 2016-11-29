using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Serilog;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class ControlDocumentFilesExists : INoark5Test
    {
        private readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TestRun _testRun;
        private readonly DirectoryInfo _workingDirectory;

        public ControlDocumentFilesExists(Archive archive)
        {
            _testRun = new TestRun(GetName(), TestType.ContentControl);
            _workingDirectory = archive.WorkingDirectory;
        }

        public string GetName()
        {
            return Noark5Messages.ControlDocumentFilesExists;
        }

        public TestRun GetTestRun()
        {
            return _testRun;
        }

        public void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        public void OnReadEndElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        public void OnReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("referanseDokumentfil"))
            {
                string documentFileName = eventArgs.Value;
                if (!FileExists(documentFileName))
                {
                    _testRun.Add(new TestResult(ResultType.Error, new Location(documentFileName),
                        Noark5Messages.ControlDocumentsFilesExistsMessage1));
                }
            }
        }

        /// <summary>
        /// Checks if file exists on disk. 
        /// IMPORTANT - DO NOT PERFORM LOGGING ON EACH FILE EXISTS CHECK - IT KILLS PERFORMANCE!
        /// </summary>
        /// <param name="documentFileName"></param>
        /// <returns></returns>
        private bool FileExists(string documentFileName)
        {
            var file = new FileInfo(Path.Combine(_workingDirectory.FullName, documentFileName));
            bool fileExists = file.Exists;
            return fileExists;
        }
    }
}