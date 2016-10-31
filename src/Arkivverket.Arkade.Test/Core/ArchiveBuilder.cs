using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Core
{
    public class ArchiveBuilder
    {
        private ArchiveType _archiveType = ArchiveType.Noark5;

        private Uuid _uuid = Uuid.Random();
        private DirectoryInfo _workingDirectory = new DirectoryInfo("c:\\temp");

        public ArchiveBuilder WithUuid(string uuid)
        {
            _uuid = Uuid.Of(uuid);
            return this;
        }

        public ArchiveBuilder WithUuid(Uuid uuid)
        {
            _uuid = uuid;
            return this;
        }

        public ArchiveBuilder WithWorkingDirectory(string workingDirectory)
        {
            _workingDirectory = new DirectoryInfo(workingDirectory);
            return this;
        }

        public ArchiveBuilder WithWorkingDirectory(DirectoryInfo workingDirectory)
        {
            _workingDirectory = workingDirectory;
            return this;
        }

        public ArchiveBuilder WithArchiveType(ArchiveType archiveType)
        {
            _archiveType = archiveType;
            return this;
        }

        public Archive Build()
        {
            var archive = new Archive(_archiveType, _uuid, _workingDirectory);
            return archive;
        }
    }
}