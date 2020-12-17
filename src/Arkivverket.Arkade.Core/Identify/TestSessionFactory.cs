using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Serilog;

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
            WorkingDirectory workingDirectory = WorkingDirectory.FromUuid(uuid, archiveDirectory.Directory);
            
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

            WorkingDirectory workingDirectory = WorkingDirectory.FromUuid(uuid);

            if (archiveFile.ArchiveType == ArchiveType.Siard && archiveFile.File.Extension.Equals(".siard"))
            {
                File.Copy(archiveFile.File.FullName,
                    Path.Combine(workingDirectory.Content().ToString(), archiveFile.File.Name));
            }
            else
            {
                TarExtractionStartedEvent();
                _compressionUtility.ExtractFolderFromArchive(archiveFile.File, workingDirectory.Root().DirectoryInfo());
                TarExtractionFinishedEvent(workingDirectory);
            }

            TestSession testSession = NewSession(workingDirectory, archiveFile.ArchiveType, uuid);

            ReadingArchiveFinishedEvent();
            return testSession;
        }
        private TestSession NewSession(WorkingDirectory workingDirectory, ArchiveType archiveType, Uuid uuid)
        {
            Archive archive = new Archive(archiveType, uuid, workingDirectory);

            workingDirectory.CopyAddmlFileToAdministrativeMetadata();

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

            ArchiveXmlFile addmlFile = archive.AddmlXmlUnit.File;

            try
            {
                AddmlInfo addml = AddmlUtil.ReadFromFile(addmlFile.FullName);

                var addmlDefinitionParser = new AddmlDefinitionParser(addml, workingDirectory, _statusEventHandler);

                testSession.AddmlDefinition = addmlDefinitionParser.GetAddmlDefinition();
            }
            catch (Exception exception)
            {
                var message = "Reading file " + addmlFile.Name + " failed: " + exception.Message;
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
                Resources.Messages.ReadingArchiveEvent,
                Resources.Messages.TarExtractionMessageStarted, OperationMessageStatus.Started);
        }

        private void TarExtractionFinishedEvent(WorkingDirectory workingDirectory)
        {
            _statusEventHandler.RaiseEventOperationMessage(Resources.Messages.ReadingArchiveEvent,
                string.Format(Resources.Messages.TarExtractionMessageFinished, workingDirectory.ContentWorkDirectory().DirectoryInfo().FullName),
                OperationMessageStatus.Ok);
        }
    }
}