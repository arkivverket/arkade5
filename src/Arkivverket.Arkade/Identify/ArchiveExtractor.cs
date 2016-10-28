using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractor : IArchiveExtractor
    {
        private readonly ILogger _log = Log.ForContext<ArchiveExtractor>();

        private readonly ICompressionUtility _compressionUtility;

        public ArchiveExtractor(ICompressionUtility compressionUtility)
        {
            _compressionUtility = compressionUtility;
        }

        public DirectoryInfo Extract(FileInfo fileName)
        {
            var uuid = Uuid.Of(Path.GetFileNameWithoutExtension(fileName.Name));
            var targetFolderName = ArkadeConstants.GetArkadeTempDirectory().FullName + Path.DirectorySeparatorChar + uuid.GetValue();

            if (Directory.Exists(targetFolderName))
            {
                Directory.Delete(targetFolderName, true);
                _log.Information("Removed folder {}", targetFolderName);
            }

            _compressionUtility.ExtractFolderFromArchive(fileName.FullName, targetFolderName);


            return new DirectoryInfo(targetFolderName);
        }
    }
}