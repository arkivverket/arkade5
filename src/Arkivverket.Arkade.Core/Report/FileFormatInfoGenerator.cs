using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Arkivverket.Arkade.Core.Report
{
    public static class FileFormatInfoGenerator
    {
        private static readonly List<FileTypeStatisticsElement> AmountOfFilesPerFileType = new List<FileTypeStatisticsElement>();

        public static void Generate(DirectoryInfo filesDirectory, string resultFileFullPath, bool filesAreReferencedFromFilesDirectoryParent = false)
        {
            var listElements = new List<ListElement>();

            IEnumerable<SiegfriedFileInfo> siegfriedFileInfoSet = GetFormatInfoAllFiles(filesDirectory);

            DirectoryInfo startDirectory = filesAreReferencedFromFilesDirectoryParent
                ? filesDirectory.Parent
                : filesDirectory;

            ArrangeFileFormatStatistics(siegfriedFileInfoSet, startDirectory, listElements);

            WriteFileList(resultFileFullPath, listElements);

            WriteFileTypeStatisticsFile(resultFileFullPath);
        }

        private static IEnumerable<SiegfriedFileInfo> GetFormatInfoAllFiles(DirectoryInfo directory)
        {
            var fileFormatIdentifier = new SiegfriedFileFormatIdentifier();

            return fileFormatIdentifier.IdentifyFormat(directory);
        }

        private static void ArrangeFileFormatStatistics(IEnumerable<SiegfriedFileInfo> siegfriedFileInfoSet,
            DirectoryInfo startDirectory, List<ListElement> listElements)
        {
            foreach (SiegfriedFileInfo siegfriedFileInfo in siegfriedFileInfoSet)
            {
                string fileName = startDirectory?.Parent != null
                    ? Path.GetRelativePath(startDirectory.Parent.FullName, siegfriedFileInfo.FileName)
                    : siegfriedFileInfo.FileName;

                var documentFileListElement = new ListElement
                {
                    FileName = fileName.Replace('\\','/'),
                    FileExtension = siegfriedFileInfo.FileExtension,
                    FileFormatPuId = siegfriedFileInfo.Id,
                    FileFormatName = siegfriedFileInfo.Format,
                    FileFormatVersion = siegfriedFileInfo.Version,
                    FileMimeType = siegfriedFileInfo.MimeType,
                    FileScanError = siegfriedFileInfo.Errors,
                };

                listElements.Add(documentFileListElement);

                string key = documentFileListElement.FileFormatPuId + " - " + documentFileListElement.FileFormatName;

                var fileTypeStatistic = new FileTypeStatisticsElement
                {
                    FileType = key,
                    Amount = 1,
                };

                FileTypeStatisticsElement existingStat = AmountOfFilesPerFileType.Find(t => t.FileType.Equals(key));

                if (existingStat == null)
                    AmountOfFilesPerFileType.Add(fileTypeStatistic);
                else
                    existingStat.Amount++;

            }
        }

        private static void WriteFileList(string fullFileName, List<ListElement> listElements)
        {
            using (var writer = new StreamWriter(fullFileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listElements);
            }
        }

        private static void WriteFileTypeStatisticsFile(string fileFormatInfoFileName)
        {
            string fullFileName = Path.Combine(Path.GetDirectoryName(fileFormatInfoFileName),
                string.Format(ArkadeConstants.FileFormatInfoStatisticsFileName,
                    Path.GetFileNameWithoutExtension(fileFormatInfoFileName)));

            using (var writer = new StreamWriter(fullFileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(AmountOfFilesPerFileType);
                }
            }
        }

        private class ListElement
        {
            [Name(ArkadeConstants.FileFormatInfoHeaders.FileName)]
            public string FileName { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.FileExtension)]
            public string FileExtension { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.FormatId)]
            public string FileFormatPuId { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.FormatName)]
            public string FileFormatName { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.FormatVersion)]
            public string FileFormatVersion { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.MimeType)]
            public string FileMimeType { get; set; }

            [Name(ArkadeConstants.FileFormatInfoHeaders.FileScanError)]
            public string FileScanError { get; set; }
        }

        private class FileTypeStatisticsElement
        {
            [Name(ArkadeConstants.FileFormatInfoStatisticsHeaders.FileType)]
            public string FileType { get; set; }

            [Name(ArkadeConstants.FileFormatInfoStatisticsHeaders.Amount)]
            public int Amount { get; set; }
        }
    }
}
