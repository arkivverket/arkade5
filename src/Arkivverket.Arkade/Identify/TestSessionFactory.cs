using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Test.Core;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class TestSessionFactory : ITestSessionFactory
    {
        private readonly IArchiveExtractor _archiveExtractor;
        private readonly IArchiveIdentifier _archiveIdentifier;
        private readonly ILogger _log = Log.ForContext<TestSessionFactory>();
        private readonly IStatusEventHandler _statusEventHandler;

        public TestSessionFactory(IArchiveExtractor archiveExtractor, IArchiveIdentifier archiveIdentifier,
            StatusEventHandler statusEventHandler)
        {
            _archiveExtractor = archiveExtractor;
            _archiveIdentifier = archiveIdentifier;
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName)
        {
            _log.Information(
                $"Building new TestSession with [archiveFileName: {archiveFileName}] [metadataFileName: {metadataFileName}");
            FileInfo archiveFileInfo = new FileInfo(archiveFileName);

            var uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFileName));
            var archiveType = _archiveIdentifier.Identify(metadataFileName);

            _statusEventHandler.IssueOnTestInformation(Resources.Messages.TarExtractionMessage,
                $"Starter utpakking av {archiveFileName}", StatusTestExecution.TestStarted, false);

            DirectoryInfo targetFolderName = _archiveExtractor.Extract(archiveFileInfo);
            Archive archive = new Archive(archiveType, uuid, targetFolderName);

            ConvertNoarkihToAddmlIfNoark4(archive);

            _statusEventHandler.IssueOnTestInformation(Resources.Messages.TarExtractionMessage,
                $"{archiveFileName} er ferdig utpakket", StatusTestExecution.TestCompleted, true);

            var testSession = new TestSession(archive);
            if (archiveType != ArchiveType.Noark5)
            {
                AddmlInfo addml = AddmlUtil.ReadFromFile(archive.GetStructureDescriptionFileName());
                testSession.AddmlDefinition = new AddmlDefinitionParser(addml).GetAddmlDefinition();
            }

            return testSession;
        }


        private void ConvertNoarkihToAddmlIfNoark4(Archive archive)
        {
            if (archive.ArchiveType != ArchiveType.Noark4)
            {
                return;
            }

            string addmlFile =
                archive.WorkingDirectory.FullName + Path.DirectorySeparatorChar + ArkadeConstants.AddmlXmlFileName;

            if (File.Exists(addmlFile))
            {
                _log.Information("{0} already exists. XSLT transformation of {1} skipped.",
                    ArkadeConstants.AddmlXmlFileName, ArkadeConstants.NoarkihXmlFileName);
                return;
            }

            string noarkihFile =
                archive.WorkingDirectory.FullName + Path.DirectorySeparatorChar + ArkadeConstants.NoarkihXmlFileName;
            if (!File.Exists(noarkihFile))
            {
                throw new ArkadeException("No such file: " + noarkihFile);
            }

            string noarkihString = File.ReadAllText(noarkihFile);
            try
            {
                string addmlString = NoarkihToAddmlTransformer.Transform(noarkihString);

                // Verify correct ADDML file
                AddmlUtil.ReadFromString(addmlString);

                File.WriteAllText(addmlFile, addmlString);
                _log.Information("Successfully transformed {0} to {1}.", ArkadeConstants.NoarkihXmlFileName,
                    ArkadeConstants.AddmlXmlFileName);
            }
            catch (Exception e)
            {
                throw new ArkadeException("Unable to convert " + ArkadeConstants.NoarkihXmlFileName + " to " + ArkadeConstants.InfoXmlFileName, e);
            }
        }
    }
}