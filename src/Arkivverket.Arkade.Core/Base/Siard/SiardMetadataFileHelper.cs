using System.IO;
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

        public void ExtractSiardMetadataFilesToAdministrativeMetadata(Archive archive)
        {
            var administrativeMetadataPath = archive.WorkingDirectory.AdministrativeMetadata().ToString();
            string archiveFilePath =
                archive.WorkingDirectory.Content().DirectoryInfo().GetFiles("*.siard")[0].FullName;
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
