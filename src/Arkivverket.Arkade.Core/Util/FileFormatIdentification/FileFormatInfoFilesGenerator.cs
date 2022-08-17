using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Resources;
using CsvHelper.Configuration;
using static Arkivverket.Arkade.Core.Util.CsvHelper;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class FileFormatInfoFilesGenerator : IFileFormatInfoFilesGenerator
    {
        public void Generate(IEnumerable<IFileFormatInfo> fileFormatInfoSet, string relativePathRoot, string resultFileFullPath)
        {
            (List<ListElement> listElements, List<FileTypeStatisticsElement> fileTypeStatisticsElements) =
                ArrangeFileFormatStatistics(fileFormatInfoSet, relativePathRoot);

            WriteFileList(resultFileFullPath, listElements);

            WriteFileTypeStatisticsFile(resultFileFullPath, fileTypeStatisticsElements);
        }

        private (List<ListElement>, List<FileTypeStatisticsElement>) ArrangeFileFormatStatistics(
            IEnumerable<IFileFormatInfo> fileFormatInfoSet, string relativePathRoot)
        {
            var listElements = new List<ListElement>();
            var fileTypeStatisticsElements = new List<FileTypeStatisticsElement>();

            foreach (IFileFormatInfo fileFormatInfo in fileFormatInfoSet)
            {
                if (fileFormatInfo == null)
                    continue;

                string fileName = Path.IsPathFullyQualified(fileFormatInfo.FileName)
                    ? Path.GetRelativePath(relativePathRoot, fileFormatInfo.FileName)
                    : fileFormatInfo.FileName;

                var documentFileListElement = new ListElement
                {
                    FileName = fileName.Replace('\\', '/'),
                    FileExtension = fileFormatInfo.FileExtension,
                    FileFormatPuId = fileFormatInfo.Id,
                    FileFormatName = fileFormatInfo.Format,
                    FileFormatVersion = fileFormatInfo.Version,
                    FileMimeType = fileFormatInfo.MimeType,
                    FileScanError = fileFormatInfo.Errors,
                };

                listElements.Add(documentFileListElement);

                string key = documentFileListElement.FileFormatPuId +
                             (string.IsNullOrWhiteSpace(documentFileListElement.FileFormatName)
                                 ? string.Empty
                                 : $" - {documentFileListElement.FileFormatName}") +
                             (string.IsNullOrWhiteSpace(documentFileListElement.FileFormatVersion)
                                 ? string.Empty
                                 : $" - {documentFileListElement.FileFormatVersion}");

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

            fileTypeStatisticsElements.Sort();

            return (listElements, fileTypeStatisticsElements);
        }

        private static void WriteFileList(string fullFileName, IEnumerable<ListElement> listElements)
        {
            WriteToFile<ListElement, ListElementMap>(fullFileName, listElements);
        }

        private static void WriteFileTypeStatisticsFile(string fileFormatInfoFileName,
            IEnumerable<FileTypeStatisticsElement> fileTypeStatisticsElements)
        {
            string fullFileName = Path.Combine(Path.GetDirectoryName(fileFormatInfoFileName),
                string.Format(OutputFileNames.FileFormatInfoStatisticsFile,
                    Path.GetFileNameWithoutExtension(fileFormatInfoFileName)));

            WriteToFile<FileTypeStatisticsElement, FileTypeStatisticsElementMap>(fullFileName, fileTypeStatisticsElements);
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

        private class FileTypeStatisticsElement : IComparable
        {
            public string FileType { get; set; }
            public int Amount { get; set; }
            
            public int CompareTo(object obj)
            {
                return obj is not FileTypeStatisticsElement other
                    ? 1
                    : string.Compare(FileType, other.FileType, StringComparison.InvariantCulture);
            }
        }
    }
}
