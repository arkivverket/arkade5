using System;
using System.IO;
using System.Reflection.Emit;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Moq;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    public class ArchiveBuilder(IArchiveContent content, DirectoryInfo processingDirectory)
    {
        private IArchiveContent _content = content;
        private DirectoryInfo _processingDirectory = processingDirectory;
        private InputDiasPackage _inputDiasPackage;
        
        private ArchiveType _archiveType = ArchiveType.Noark5;
        private ArchiveDetails _archiveDetails;

        private Uuid _uuid = Uuid.Random(); // NB! UUID-origin
        private DirectoryInfo _workingDirectoryContent;
        private DirectoryInfo _workingDirectory;
        private string _archiveFileFullName;

        public ArchiveBuilder WithUuid(string uuid)
        {
            Uuid.TryParse(uuid, out _uuid);
            return this;
        }

        public ArchiveBuilder WithUuid(Uuid uuid)
        {
            _uuid = uuid; // NB! UUID-transfer
            return this;
        }

        public ArchiveBuilder WithWorkingDirectoryRoot(string workingDirectory)
        {
            _workingDirectory = new DirectoryInfo(workingDirectory);
            return this;
        }

        public ArchiveBuilder WithWorkingDirectoryExternalContent(string workingDirectory)
        {
            _workingDirectoryContent = new DirectoryInfo(workingDirectory);
            return this;
        }

        public ArchiveBuilder WithWorkingDirectoryExternalContent(DirectoryInfo workingDirectory)
        {
            _workingDirectoryContent = workingDirectory;
            return this;
        }

        public ArchiveBuilder WithArchiveType(ArchiveType archiveType)
        {
            _archiveType = archiveType;
            return this;
        }

        // public ArchiveBuilder WithArchiveDetails(string standardVersion)
        // {
        //     var mock = new Mock<ArchiveDetails>(Build().AddmlInfo.Addml);
        //     mock.Setup(x => x.ArchiveStandard).Returns(standardVersion);
        //     _archiveDetails = mock.Object;
        //     return this;
        // }

        public ArchiveBuilder WithProcessingDirectory()
        {
            _processingDirectory = new DirectoryInfo(Path.Combine(_workingDirectory.FullName, _uuid.ToString())); // øh ..
            
            return this;
        }
        
        public ArchiveBuilder WithContent<T>(string pathToContent) where T : IArchiveContent
        {
            _content = typeof(T) switch
            {
                var t when t == typeof(DirectoryArchiveContent)
                    => new Mock<DirectoryArchiveContent>(new DirectoryInfo(pathToContent)).Object,
                
                var t when t == typeof(FileArchiveContent)
                    => new Mock<FileArchiveContent>(new FileInfo(pathToContent)).Object,
                
                _ => throw new ArgumentException($"Unknown content type: {typeof(T)}")
            };

            return this;
        }

        public ArchiveBuilder WithInputDiasPackage(ArchiveMetadata archiveMetadata)
        {
            var mock = new Mock<InputDiasPackage>(); //_uuid, null!, null!);
            //mock.Setup(x => x.ArchiveMetadata).Returns(archiveMetadata);
            _inputDiasPackage = mock.Object;
            
            return this;
        }

        public Archive Build<T>() where T : Archive
        {
            if (_content == null)
                throw new Exception("Content is required for all types of archives");
            
            // if (_processingDirectory == null)
            //     throw new Exception("ProcessingDirectory is required for all types of archives");
            
            return (Archive)Activator.CreateInstance(typeof(T), _content, _processingDirectory, _inputDiasPackage);
        }
    }
}
