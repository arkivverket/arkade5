using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class TestSessionFactory : ITestSessionFactory
    {
        private readonly ICompressionUtility _compressionUtility;
        private readonly ILogger _log = Log.ForContext<TestSessionFactory>();
        private readonly IStatusEventHandler _statusEventHandler;

        public TestSessionFactory(ICompressionUtility compressionUtility, IStatusEventHandler statusEventHandler)
        {
            _compressionUtility = compressionUtility;
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSession(ArchiveDirectory archiveDirectory)
        {
            ReadingArchiveStartedEvent();

            ArchiveType archiveType = archiveDirectory.ArchiveType;
            _log.Information(
                $"Building new TestSession from directory [archiveType: {archiveType}] [directory: {archiveDirectory.Directory.FullName}]");

            Uuid uuid = Uuid.Random();
            ArchiveInformationEvent(archiveDirectory.Directory.FullName, archiveType, uuid);
            WorkingDirectory workingDirectory = WorkingDirectory.FromUuid(uuid, archiveDirectory.Directory);
            
            TestSession testSession = NewSession(workingDirectory, archiveType, uuid);

            ReadingArchiveFinishedEvent();
            return testSession;
        }

        public TestSession NewSession(ArchiveFile archiveFile)
        {
            ReadingArchiveStartedEvent();
            _log.Information(
                $"Building new TestSession from file [archiveType: {archiveFile.ArchiveType}] [directory: {archiveFile.File.FullName}]");
            Uuid uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFile.File.Name));
            ArchiveInformationEvent(archiveFile.File.FullName, archiveFile.ArchiveType, uuid);

            WorkingDirectory workingDirectory = WorkingDirectory.FromUuid(uuid);

            TarExtractionStartedEvent();
            _compressionUtility.ExtractFolderFromArchive(archiveFile.File, workingDirectory.Root().DirectoryInfo());
            TarExtractionFinishedEvent(workingDirectory);

            TestSession testSession = NewSession(workingDirectory, archiveFile.ArchiveType, uuid);

            ReadingArchiveFinishedEvent();
            return testSession;
        }
        private TestSession NewSession(WorkingDirectory workingDirectory, ArchiveType archiveType, Uuid uuid)
        {
            Archive archive = new Archive(archiveType, uuid, workingDirectory);

            workingDirectory.CopyAddmlFileToAdministrativeMetadata();

            ConvertNoarkihToAddmlIfNoark4(archive);

            var testSession = new TestSession(archive);
            if (archiveType != ArchiveType.Noark5)
            {
                AddmlInfo addml = AddmlUtil.ReadFromFile(archive.GetStructureDescriptionFileName());
                testSession.AddmlDefinition = new AddmlDefinitionParser(addml, workingDirectory, _statusEventHandler).GetAddmlDefinition();
            }

            return testSession;
        }

        private void ArchiveInformationEvent(string archiveFileName, ArchiveType archiveType, Uuid uuid)
        {
            _statusEventHandler.RaiseEventNewArchiveInformation(new ArchiveInformationEventArgs(
                archiveType.ToString(), uuid.ToString(), archiveFileName));
        }

        private void ReadingArchiveStartedEvent()
        {
            
        }

        private void ReadingArchiveFinishedEvent()
        {

        }

        private void TarExtractionStartedEvent()
        {
            _statusEventHandler.RaiseEventOperationMessage(
                Resources.Messages.ReadingArchiveEvent,
                Resources.Messages.TarExtractionMessageStarted, OperationMessageStatus.Started);
        }

        private void TarExtractionFinishedEvent(WorkingDirectory workingDirectory)
        {
            _statusEventHandler.RaiseEventOperationMessage(Resources.Messages.ReadingArchiveEvent,
                string.Format(Resources.Messages.TarExtractionMessageFinished, workingDirectory.ContentWorkDirectory().DirectoryInfo().FullName),
                OperationMessageStatus.Ok);
        }

        private void ConvertNoarkihToAddmlIfNoark4(Archive archive)
        {
            if (archive.ArchiveType != ArchiveType.Noark4)
            {
                return;
            }

            FileInfo addmlFileInContentFolder = archive.WorkingDirectory.Content().WithFile(ArkadeConstants.AddmlXmlFileName);

            if (addmlFileInContentFolder.Exists)
            {
                _log.Information("{0} already exists. XSLT transformation of {1} skipped.",
                    ArkadeConstants.AddmlXmlFileName, ArkadeConstants.NoarkihXmlFileName);
                return;
            }

            FileInfo noarkihFile = archive.WorkingDirectory.Content().WithFile(ArkadeConstants.NoarkihXmlFileName);

            if (!noarkihFile.Exists)
            {
                throw new ArkadeException(string.Format(Resources.Messages.FileNotFoundMessage, noarkihFile.FullName));
            }

            string noarkihString = File.ReadAllText(noarkihFile.FullName);
            try
            {
                // TODO: Use stream instead of strings
                string addmlString = NoarkihToAddmlTransformer.Transform(noarkihString);

                // Verify correct ADDML file
                AddmlUtil.ReadFromString(addmlString);

                FileInfo addmlFileToWrite = archive.WorkingDirectory.AdministrativeMetadata().WithFile(ArkadeConstants.AddmlXmlFileName);

                File.WriteAllText(addmlFileToWrite.FullName, addmlString);
                _log.Information("Successfully transformed {0} to {1}.", ArkadeConstants.NoarkihXmlFileName,
                    ArkadeConstants.AddmlXmlFileName);
            }
            catch (Exception e)
            {
                string message = string.Format(Resources.Messages.Noark4ConvertNoarkihFileError, 
                    ArkadeConstants.NoarkihXmlFileName,
                    ArkadeConstants.AddmlXmlFileName);

                throw new ArkadeException(message,e);
            }
        }

    }
}