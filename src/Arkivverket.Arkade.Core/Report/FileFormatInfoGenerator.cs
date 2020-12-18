using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Serilog;

namespace Arkivverket.Arkade.Core.Report
{
    public static class FileFormatInfoGenerator
    {
        public static void Generate(DirectoryInfo filesDirectory, string resultFileFullPath,
            bool filesAreReferencedFromFilesDirectoryParent = false)
        {
            Log.Information($"Starting file format analysis.");

            IEnumerable<IFileFormatInfo> siegfriedFileInfoSet = GetFormatInfoAllFiles(filesDirectory);

            DirectoryInfo startDirectory = filesAreReferencedFromFilesDirectoryParent
                ? filesDirectory.Parent
                : filesDirectory;

            (List<ListElement> listElements, List<FileTypeStatisticsElement> fileTypeStatisticsElements) =
                ArrangeFileFormatStatistics(siegfriedFileInfoSet, startDirectory);

            WriteFileList(resultFileFullPath, listElements);

            WriteFileTypeStatisticsFile(resultFileFullPath, fileTypeStatisticsElements);

            Log.Information($"File format analysis completed.");
        }

        public static void Generate(IEnumerable<IFileFormatInfo> siegfriedFileInfoSet, string siardFolder, string resultFileFullPath)
        {
            (List<ListElement> listElements, List<FileTypeStatisticsElement> fileTypeStatisticsElements) =
                ArrangeFileFormatStatistics(siegfriedFileInfoSet, new DirectoryInfo(siardFolder));

            WriteFileList(resultFileFullPath, listElements);

            WriteFileTypeStatisticsFile(resultFileFullPath, fileTypeStatisticsElements);
        }

        private static IEnumerable<IFileFormatInfo> GetFormatInfoAllFiles(DirectoryInfo directory)
        {
            var fileFormatIdentifier = new SiegfriedFileFormatIdentifier();

            return fileFormatIdentifier.IdentifyFormat(directory);
        }

        private static (List<ListElement>, List<FileTypeStatisticsElement>) ArrangeFileFormatStatistics(
            IEnumerable<IFileFormatInfo> siegfriedFileInfoSet, DirectoryInfo startDirectory)
        {
            var listElements = new List<ListElement>();
            var fileTypeStatisticsElements = new List<FileTypeStatisticsElement>();

            foreach (IFileFormatInfo siegfriedFileInfo in siegfriedFileInfoSet)
            {
                if (siegfriedFileInfo == null)
                    continue;

                string fileName = Path.IsPathFullyQualified(siegfriedFileInfo.FileName)
                    ? Path.GetRelativePath(startDirectory.FullName, siegfriedFileInfo.FileName)
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

                var fileTypeStatisticElement = new FileTypeStatisticsElement
                {
                    FileType = key,
                    Amount = 1,
                };

                FileTypeStatisticsElement existingStat = fileTypeStatisticsElements.Find(t => t.FileType.Equals(key));

                if (existingStat == null)
                    fileTypeStatisticsElements.Add(fileTypeStatisticElement);
                else
                    existingStat.Amount++;
            }

            return (listElements, fileTypeStatisticsElements);
        }

        private static void WriteFileList(string fullFileName, List<ListElement> listElements)
        {
            using (var writer = new StreamWriter(fullFileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listElements);
            }
        }

        private static void WriteFileTypeStatisticsFile(string fileFormatInfoFileName,
            List<FileTypeStatisticsElement> fileTypeStatisticsElements)
        {
            string fullFileName = Path.Combine(Path.GetDirectoryName(fileFormatInfoFileName),
                string.Format(ArkadeConstants.FileFormatInfoStatisticsFileName,
                    Path.GetFileNameWithoutExtension(fileFormatInfoFileName)));

            using (var writer = new StreamWriter(fullFileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(fileTypeStatisticsElements);
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
