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

        public TestSession NewSession(ArchiveDirectory archiveDirectory) // TODO: Remake as Archive-factory?
        {
            ArchiveType archiveType = archiveDirectory.ArchiveType;
            _log.Debug(
                $"Building new TestSession from directory [archiveType: {archiveType}] [directory: {archiveDirectory.Directory.FullName}]");

            ArchiveInformationEvent(archiveDirectory.Directory.FullName, archiveType);
            WorkingDirectory workingDirectory = WorkingDirectory.FromExternalDirectory(archiveDirectory.Directory);

            ArkadeDirectory content = null; // TODO: Provide

            var archive = new Archive(archiveType, workingDirectory, content, _statusEventHandler); // ...

            TestSession testSession = NewSession(archive);

            return testSession;
        }

        public TestSession NewSession(ArchiveFile archiveFile) // TODO: Remake as Archive-factory?
        {
            _log.Debug(
                $"Building new TestSession from file [archiveType: {archiveFile.ArchiveType}] [directory: {archiveFile.File.FullName}]");

            Uuid.TryParse(Path.GetFileNameWithoutExtension(archiveFile.File.Name), out Uuid inputDiasPackageId); // NB! UUID-orig
            
            ArchiveInformationEvent(archiveFile.File.FullName, archiveFile.ArchiveType, inputDiasPackageId);

            WorkingDirectory workingDirectory = WorkingDirectory.FromArchiveFile();

            ArkadeDirectory content = null; // TODO: Provide

            if (archiveFile.ArchiveType == ArchiveType.Siard && archiveFile.File.Extension.Equals(".siard"))
            {
                CopySiardFilesToContentDirectory(archiveFile, content.ToString());
            }
            else
            {
                TarExtractionStartedEvent();
                _compressionUtility.ExtractFolderFromArchive(archiveFile.File, workingDirectory.Root().DirectoryInfo(),
                    withoutDocumentFiles: archiveFile.ArchiveType == ArchiveType.Noark5, archiveRootDirectoryName: inputDiasPackageId?.ToString());
                TarExtractionFinishedEvent(workingDirectory);
            }

            var archive = new Archive(archiveFile.ArchiveType, workingDirectory, content, _statusEventHandler, archiveFile.File.FullName); // ...

            TestSession testSession = NewSession(archive, inputDiasPackageId);

            return testSession;
        }

        private TestSession NewSession(Archive archive, Uuid inputDiasPackageId = null)
        {

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
                archive.WorkingDirectory.EnsureAdministrativeMetadataHasAddmlFiles(archive.AddmlXmlUnit.File.Name);

            var testSession = new TestSession(archive, inputDiasPackageId);

            if (archive.ArchiveType is ArchiveType.Noark5 or ArchiveType.Siard)
            {
                return testSession;
            }

            AddmlInfo addml = archive.AddmlInfo;

            try
            {
                var addmlDefinitionParser = new AddmlDefinitionParser(addml, archive.Content, _statusEventHandler);

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

        private void ArchiveInformationEvent(string archiveFileName, ArchiveType archiveType, Uuid inputDiasPackageUuid = null)
        {
            _statusEventHandler.RaiseEventNewArchiveInformation(new ArchiveInformationEventArgs(
                archiveType.ToString(), inputDiasPackageUuid?.ToString()?? "-", archiveFileName)); // NB! UUID-writeout (right after UUID init)
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