using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Identify
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
            _log.Debug(
                $"Building new TestSession from directory [archiveType: {archiveType}] [directory: {archiveDirectory.Directory.FullName}]");

            Uuid uuid = Uuid.Random();
            ArchiveInformationEvent(archiveDirectory.Directory.FullName, archiveType, uuid);
            WorkingDirectory workingDirectory = WorkingDirectory.FromExternalDirectory(archiveDirectory.Directory);
            
            TestSession testSession = NewSession(workingDirectory, archiveType, uuid);

            ReadingArchiveFinishedEvent();
            return testSession;
        }

        public TestSession NewSession(ArchiveFile archiveFile)
        {
            ReadingArchiveStartedEvent();
            _log.Debug(
                $"Building new TestSession from file [archiveType: {archiveFile.ArchiveType}] [directory: {archiveFile.File.FullName}]");
            Uuid uuid = archiveFile.File.Extension.Equals(".siard")
                ? Uuid.Random()
                : Uuid.Of(Path.GetFileNameWithoutExtension(archiveFile.File.Name));
            ArchiveInformationEvent(archiveFile.File.FullName, archiveFile.ArchiveType, uuid);

            WorkingDirectory workingDirectory = WorkingDirectory.FromArchiveFile();

            if (archiveFile.ArchiveType == ArchiveType.Siard && archiveFile.File.Extension.Equals(".siard"))
            {
                CopySiardFilesToContentDirectory(archiveFile, workingDirectory.Content().ToString());
            }
            else
            {
                TarExtractionStartedEvent();
                _compressionUtility.ExtractFolderFromArchive(archiveFile.File, workingDirectory.Root().DirectoryInfo(),
                    withoutDocumentFiles: true, archiveRootDirectoryName: uuid.ToString());
                TarExtractionFinishedEvent(workingDirectory);
            }

            TestSession testSession = NewSession(workingDirectory, archiveFile.ArchiveType, uuid, archiveFile.File.FullName);

            ReadingArchiveFinishedEvent();
            return testSession;
        }

        private TestSession NewSession(WorkingDirectory workingDirectory, ArchiveType archiveType, Uuid uuid,
            string archiveFileFullName = null)
        {
            Archive archive = new Archive(archiveType, uuid, workingDirectory, _statusEventHandler, archiveFileFullName);

            if (archive.ArchiveType == ArchiveType.Noark5 && archive.AddmlXmlUnit.File.Exists &&
                archive.AddmlXmlUnit.Schema.IsArkadeBuiltIn())
            {
                _statusEventHandler?.RaiseEventOperationMessage(
                    Noark5Messages.MissingAddmlSchema,
                    string.Format(Noark5Messages.UsingBuiltInAddmlSchemaFile, BuiltInAddmlSchemaVersion),
                    OperationMessageStatus.Warning);
                Log.Warning(string.Format(Noark5Messages.InternalSchemaFileIsUsed,
                    AddmlXsdFileName, BuiltInAddmlSchemaVersion));
            }

            if (archive.ArchiveType is ArchiveType.Noark5 or ArchiveType.Fagsystem)
                workingDirectory.EnsureAdministrativeMetadataHasAddmlFiles(archive.AddmlXmlUnit.File.Name);

            var testSession = new TestSession(archive);

            if (archiveType == ArchiveType.Noark5)
            {
                testSession.AvailableTests = Noark5TestProvider.GetAllTestIds();

                return testSession;
            }

            if (archiveType == ArchiveType.Siard)
            {
                return testSession;
            }

            AddmlInfo addml = archive.AddmlInfo;

            try
            {
                var addmlDefinitionParser = new AddmlDefinitionParser(addml, workingDirectory, _statusEventHandler);

                testSession.AddmlDefinition = addmlDefinitionParser.GetAddmlDefinition();
            }
            catch (Exception exception)
            {
                var message = string.Format(ExceptionMessages.FileNotRead, archive.AddmlXmlUnit.File.Name) + " " + exception.Message;
                _log.Warning(message);//exception, message);
                _statusEventHandler.RaiseEventOperationMessage(null, message, OperationMessageStatus.Error);
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
                Messages.ReadingArchiveEvent,
                Messages.TarExtractionMessageStarted, OperationMessageStatus.Started);
        }

        private void TarExtractionFinishedEvent(WorkingDirectory workingDirectory)
        {
            _statusEventHandler.RaiseEventOperationMessage(Messages.ReadingArchiveEvent,
                string.Format(Messages.TarExtractionMessageFinished, workingDirectory.ContentWorkDirectory().DirectoryInfo().FullName),
                OperationMessageStatus.Ok);
        }

        private void CopySiardFilesToContentDirectory(ArchiveFile siardArchiveFile, string contentDirectoryPath)
        {
            var siardTableXmlReader = new SiardXmlTableReader(new SiardArchiveReader());

            siardArchiveFile.File.CopyTo(Path.Combine(contentDirectoryPath, siardArchiveFile.File.Name));

            try
            {
                IEnumerable<string> fullPathsToExternalLobs =
                    siardTableXmlReader.GetFullPathsToExternalLobs(siardArchiveFile.File.FullName);

                foreach (string fullPathToExternalLob in fullPathsToExternalLobs)
                {
                    if (!File.Exists(fullPathToExternalLob))
                    {
                        string message = string.Format(SiardMessages.ExternalLobFileNotFoundMessage, fullPathToExternalLob);
                        _statusEventHandler.RaiseEventOperationMessage("", message, OperationMessageStatus.Error);
                        _log.Error(message);
                        continue;
                    }

                    string relativePathFromSiardFileToExternalLob =
                        Path.GetRelativePath(siardArchiveFile.File.DirectoryName, fullPathToExternalLob);

                    string externalLobDestinationPath =
                        Path.Combine(contentDirectoryPath, relativePathFromSiardFileToExternalLob);

                    var destinationDirectoryForExternalLob = Path.GetDirectoryName(externalLobDestinationPath);

                    Directory.CreateDirectory(destinationDirectoryForExternalLob);

                    File.Copy(fullPathToExternalLob, externalLobDestinationPath);

                    _log.Debug("'{0}' has been added to Arkade temporary work area", fullPathToExternalLob);
                }
            }
            catch (SiardArchiveReaderException)
            {
                _statusEventHandler.RaiseEventOperationMessage("", SiardMessages.ExternalLobsNotCopiedWarning, OperationMessageStatus.Warning);
            }
        }
    }
}