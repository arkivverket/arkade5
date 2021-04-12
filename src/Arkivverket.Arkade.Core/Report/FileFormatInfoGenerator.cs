using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using CsvHelper;
using CsvHelper.Configuration;
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
                csv.Configuration.RegisterClassMap<ListElementMap>();
                csv.WriteRecords(listElements);
            }
        }

        private static void WriteFileTypeStatisticsFile(string fileFormatInfoFileName,
            List<FileTypeStatisticsElement> fileTypeStatisticsElements)
        {
            string fullFileName = Path.Combine(Path.GetDirectoryName(fileFormatInfoFileName),
                string.Format(OutputFileNames.FileFormatInfoStatisticsFile,
                    Path.GetFileNameWithoutExtension(fileFormatInfoFileName)));

            using (var writer = new StreamWriter(fullFileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.RegisterClassMap<FileTypeStatisticsElementMap>();
                    csv.WriteRecords(fileTypeStatisticsElements);
                }
            }
        }

        private sealed class ListElementMap : ClassMap<ListElement>
        {
            public ListElementMap()
            {
                Map(m => m.FileName).Name(FormatAnalysisResultFileContent.HeaderFileName);
                Map(m => m.FileExtension).Name(FormatAnalysisResultFileContent.HeaderFileExtension);
                Map(m => m.FileFormatPuId).Name(FormatAnalysisResultFileContent.HeaderFormatId);
                Map(m => m.FileFormatName).Name(FormatAnalysisResultFileContent.HeaderFormatName);
                Map(m => m.FileFormatVersion).Name(FormatAnalysisResultFileContent.HeaderFormatVersion);
                Map(m => m.FileMimeType).Name(FormatAnalysisResultFileContent.HeaderMimeType);
                Map(m => m.FileScanError).Name(FormatAnalysisResultFileContent.HeaderErrors);
            }
        }

        private sealed class FileTypeStatisticsElementMap : ClassMap<FileTypeStatisticsElement>
        {
            public FileTypeStatisticsElementMap()
            {
                Map(m => m.FileType).Name(FormatAnalysisResultFileContent.StatisticsHeaderFileType);
                Map(m => m.Amount).Name(FormatAnalysisResultFileContent.StatisticsHeaderAmount);
            }
        }

        private class ListElement
        {
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public string FileFormatPuId { get; set; }
            public string FileFormatName { get; set; }
            public string FileFormatVersion { get; set; }
            public string FileMimeType { get; set; }
            public string FileScanError { get; set; }
        }

        private class FileTypeStatisticsElement
        {
            public string FileType { get; set; }
            public int Amount { get; set; }
        }
    }
}
