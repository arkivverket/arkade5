using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractor : IArchiveExtractor
    {
        public const string TemporaryFolder = "c:\\temp";
        private readonly ICompressionUtility _compressionUtility;

        public ArchiveExtractor(ICompressionUtility compressionUtility)
        {
            _compressionUtility = compressionUtility;
        }

        public DirectoryInfo Extract(FileInfo fileName)
        {
            var uuid = Uuid.Of(Path.GetFileNameWithoutExtension(fileName.Name));
            var targetFolderName = TemporaryFolder + Path.DirectorySeparatorChar + uuid.GetValue();

            _compressionUtility.ExtractFolderFromArchive(fileName.FullName, targetFolderName);

            return new DirectoryInfo(targetFolderName);
        }
    }
}