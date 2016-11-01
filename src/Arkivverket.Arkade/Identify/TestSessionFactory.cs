using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Test.Core;
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
            StatusEventHandler statusEventHandler)
        {
            _compressionUtility = compressionUtility;
            _archiveIdentifier = archiveIdentifier;
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName)
        {
            _log.Information(
                $"Building new TestSession with [archiveFileName: {archiveFileName}] [metadataFileName: {metadataFileName}");
            FileInfo archiveFileInfo = new FileInfo(archiveFileName);

            Uuid uuid = Uuid.Of(Path.GetFileNameWithoutExtension(archiveFileName));
            ArchiveType archiveType = _archiveIdentifier.Identify(metadataFileName);

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
            CopyToDir(metadataFileName, workingDirectory);

            DirectoryInfo archiveExtractionDirectory = new DirectoryInfo(Path.Combine(workingDirectory, uuid.GetValue()));

            _statusEventHandler.IssueOnNewArchiveInformation(new StatusEventNewArchiveInformation(
                archiveType.ToString(), uuid.ToString(), DateTime.Now, archiveFileName));

            _statusEventHandler.IssueOnTestInformation(Resources.Messages.TarExtractionMessage,
                $"Starter utpakking av {archiveFileName}", StatusTestExecution.TestStarted, false);

            _compressionUtility.ExtractFolderFromArchive(archiveFileInfo.FullName, archiveExtractionDirectory.FullName);

            Archive archive = new Archive(archiveType, uuid, archiveExtractionDirectory);


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