using System.IO;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardMetadataFileHelper
    {
        private readonly ISiardArchiveReader _siardArchiveReader;

        public SiardMetadataFileHelper(ISiardArchiveReader siardArchiveReader)
        {
            _siardArchiveReader = siardArchiveReader;
        }

        public void ExtractSiardMetadataFilesToAdministrativeMetadata(SiardArchive archive)
        {
            var administrativeMetadataPath = archive.OutputDiasPackage.WorkingDirectory.AdministrativeMetadata().ToString();
            string archiveFilePath =
                archive.SiardFile.FullName;
            ExtractSiardMetadataFile(ArkadeConstants.SiardMetadataXmlFileName, administrativeMetadataPath,
                archiveFilePath);
            ExtractSiardMetadataFile(ArkadeConstants.SiardMetadataXsdFileName, administrativeMetadataPath,
                archiveFilePath);
        }

        private void ExtractSiardMetadataFile(string fileName, string targetDirectory, string siardFilePath)
        {
            string targetFileName = Path.Combine(targetDirectory, fileName);
            using var siardFileStream = new FileStream(siardFilePath, FileMode.Open, FileAccess.Read);
            string fileContent = _siardArchiveReader.GetNamedEntryFromSiardFileStream(siardFileStream, fileName);
            using FileStream targetFileStream = File.Create(targetFileName);
            using var streamWriter = new StreamWriter(targetFileStream, Encodings.UTF8);
            streamWriter.Write(fileContent);
        }
    }
}
