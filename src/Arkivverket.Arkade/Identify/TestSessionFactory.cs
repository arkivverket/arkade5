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
        private readonly IArchiveIdentifier _archiveIdentifier;
        private readonly ICompressionUtility _compressionUtility;
        private readonly ILogger _log = Log.ForContext<TestSessionFactory>();
        private readonly IStatusEventHandler _statusEventHandler;

        public TestSessionFactory(ICompressionUtility compressionUtility, IArchiveIdentifier archiveIdentifier,
            IStatusEventHandler statusEventHandler)
        {
            _compressionUtility = compressionUtility;
            _archiveIdentifier = archiveIdentifier;
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSessionFromArchiveDirectory(ArchiveDirectory archive)
        {
            return NewSession(archive.Archive.FullName, archive.ArchiveType, false);
        }

        public TestSession NewSessionFromArchiveFile(ArchiveFile archive)
        {
            return NewSession(archive.Archive.FullName, archive.ArchiveType, true);
        }

        private TestSession NewSession(string archiveFileName, ArchiveType archiveType, bool IsTar)
        {
            _log.Information(
                $"Building new TestSession with [archiveFileName: {archiveFileName}] [archiveType: {archiveType}]");

            TarExtractionStartedEvent();

            Uuid uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFileName));

            ArchiveInformationEvent(archiveFileName, archiveType, uuid);

            string workingDirectory = PrepareWorkingDirectory(uuid);

            DirectoryInfo archiveExtractionDirectory = new DirectoryInfo(Path.Combine(workingDirectory, uuid.GetValue()));

            // TODO: The logic in this conditional should be moved to ArchiveDirectory.ExtractToWorkDir() and ArchiveFile.ExtractToWorkDir() 
            if (IsTar)
            {
                // Extract if tar
                FileInfo archiveFileInfo = new FileInfo(archiveFileName);
                _compressionUtility.ExtractFolderFromArchive(archiveFileInfo.FullName, archiveExtractionDirectory.FullName);
            } else
            {
                // Copy recursivly if directory
                FileUtil.DirectoryCopy(archiveFileName, archiveExtractionDirectory.FullName, true);
            }

            Archive archive = new Archive(archiveType, uuid, archiveExtractionDirectory);

            ConvertNoarkihToAddmlIfNoark4(archive);

            TarExctractionFinishedEvent(workingDirectory);

            var testSession = new TestSession(archive);
            if (archiveType != ArchiveType.Noark5)
            {
                AddmlInfo addml = AddmlUtil.ReadFromFile(archive.GetStructureDescriptionFileName());
                testSession.AddmlDefinition = new AddmlDefinitionParser(addml).GetAddmlDefinition();
            }

            return testSession;
        }

        private string PrepareWorkingDirectory(Uuid uuid)
        {
            string workingDirectory = GetWorkingDirectory(uuid);
            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, true);
                _log.Information("Removed folder {}", workingDirectory);
            }
            else
            {
                Directory.CreateDirectory(workingDirectory);
            }
            return workingDirectory;
        }

        private void ArchiveInformationEvent(string archiveFileName, ArchiveType archiveType, Uuid uuid)
        {
            _statusEventHandler.RaiseEventNewArchiveInformation(new ArchiveInformationEventArgs(
                archiveType.ToString(), uuid.ToString(), archiveFileName));
        }

        private void TarExtractionStartedEvent()
        {
            _statusEventHandler.RaiseEventOperationMessage(
                Resources.Messages.ReadingArchiveEvent,
                Resources.Messages.TarExtractionMessageStarted, OperationMessageStatus.Started);
        }

        private void TarExctractionFinishedEvent(string workingDirectory)
        {
            _statusEventHandler.RaiseEventOperationMessage(Resources.Messages.ReadingArchiveEvent,
                string.Format(Resources.Messages.TarExtractionMessageFinished, workingDirectory),
                OperationMessageStatus.Ok);
        }

        private void CopyToDir(string metadataFileName, string workingDirectory)
        {
            if (metadataFileName != null)
            {
                File.Copy(metadataFileName, Path.Combine(workingDirectory, ArkadeConstants.InfoXmlFileName));
            }
        }

        private string GetWorkingDirectory(Uuid uuid)
        {
            string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
            return ArkadeConstants.GetArkadeWorkDirectory().FullName + Path.DirectorySeparatorChar + dateString + "-" +
                   uuid.GetValue();
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
                throw new ArkadeException(
                    "Unable to convert " + ArkadeConstants.NoarkihXmlFileName + " to " + ArkadeConstants.InfoXmlFileName,
                    e);
            }
        }

    }
}