using Arkivverket.Arkade.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Test.Core
{
    public class ArchiveBuilder
    {

        private Uuid _uuid = Uuid.Random();
        private string _workingDirectory = "c:\\tempp";
        private ArchiveType _archiveType = ArchiveType.Noark5;

        public ArchiveBuilder WithUuid(string uuid)
        {
            _uuid = Uuid.Of(uuid);
            return this;
        }

        public ArchiveBuilder WithWorkingDirectory(string workingDirectory)
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
            var archive = new Archive(_uuid, _workingDirectory);
            archive.ArchiveType = _archiveType;
            return archive;
        }

    }
}
