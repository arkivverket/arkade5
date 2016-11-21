using System.IO;
using System.Reflection;
using System.Xml;
using Arkivverket.Arkade.Core;
using Serilog;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class ControlDocumentFilesExists : BaseTest
    {
        private readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public ControlDocumentFilesExists(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            using (var reader = XmlReader.Create(ArchiveReader.GetContentAsStream(archive)))
            {
                while (reader.ReadToFollowing("referanseDokumentfil"))
                {
                    reader.Read(); // remember to actually read the node before trying to fetch the value...

                    string documentFileName = reader.Value;
                    if (!FileExists(documentFileName, archive.WorkingDirectory))
                    {
                        TestError(new Location(documentFileName), Resources.Messages.ControlDocumentsFilesExistsMessage1);
                    }
                }
            }
        }

        private bool FileExists(string documentFileName, DirectoryInfo archiveWorkingDirectory)
        {
            var file = new FileInfo(Path.Combine(archiveWorkingDirectory.FullName, documentFileName));
            bool fileExists = file.Exists;
            _log.Debug($"File exists? {fileExists}, path: {file.FullName}");
            return fileExists;
        }
    }
}
