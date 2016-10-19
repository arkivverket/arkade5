using Arkivverket.Arkade.Core;
using Serilog;
using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;

namespace Arkivverket.Arkade.Identify
{
    public class TestSessionFactory : ITestSessionFactory
    {
        private ILogger _log = Log.ForContext<TestSessionFactory>();

        private readonly IArchiveExtractor _archiveExtractor;
        private readonly IArchiveIdentifier _archiveIdentifier;
        private readonly IStatusEventHandler _statusEventHandler;

        public TestSessionFactory(IArchiveExtractor archiveExtractor, IArchiveIdentifier archiveIdentifier, StatusEventHandler statusEventHandler)
        {
            _archiveExtractor = archiveExtractor;
            _archiveIdentifier = archiveIdentifier;
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName)
        {
            _log.Information($"Building new TestSession with [archiveFileName: {archiveFileName}] [metadataFileName: {metadataFileName}");
            FileInfo archiveFileInfo = new FileInfo(archiveFileName);

            var uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFileName));
            var archiveType = _archiveIdentifier.Identify(metadataFileName);

            _statusEventHandler.IssueOnTestInformation(Resources.Messages.TarExtractionMessage, $"Starter utpakking av {archiveFileName}",StatusTestExecution.TestStarted, false);

            DirectoryInfo targetFolderName = _archiveExtractor.Extract(archiveFileInfo);
            Archive archive = new Archive(archiveType, uuid, targetFolderName);

            _statusEventHandler.IssueOnTestInformation(Resources.Messages.TarExtractionMessage, $"{archiveFileName} er ferdig utpakket", StatusTestExecution.TestCompleted, true);

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
