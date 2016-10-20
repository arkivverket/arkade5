using Arkivverket.Arkade.Core;
using Serilog;
using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Identify
{
    public class TestSessionFactory : ITestSessionFactory
    {
        public ILogger Log { get; set; }

        private readonly IArchiveExtractor _archiveExtractor;
        private readonly IArchiveIdentifier _archiveIdentifier;

        public TestSessionFactory(IArchiveExtractor archiveExtractor, IArchiveIdentifier archiveIdentifier)
        {
            _archiveExtractor = archiveExtractor;
            _archiveIdentifier = archiveIdentifier;
        }

        public TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName)
        {
            Log.Information($"Building new TestSession with [archiveFileName: {archiveFileName}] [metadataFileName: {metadataFileName}");
            FileInfo archiveFileInfo = new FileInfo(archiveFileName);

            var uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFileName));
            var archiveType = _archiveIdentifier.Identify(metadataFileName);

            DirectoryInfo targetFolderName = _archiveExtractor.Extract(archiveFileInfo);
            Archive archive = new Archive(archiveType, uuid, targetFolderName);

            var testSession = new TestSession(archive);
            if (archiveType != ArchiveType.Noark5)
            {
                AddmlInfo addml = AddmlUtil.ReadFromFile(archive.GetStructureDescriptionFileName());
                testSession.AddmlDefinition = new AddmlDefinitionParser(addml).GetAddmlDefinition();
            }
            return testSession;
        }
    }
}