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
        private static string _regulationVersion;

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

            ArchiveFileFormats archiveFileFormats = FileFormatsJsonParser.ParseArchiveFileFormats();
            HashSet<string> approvedPuidArchiveFormats = ArchiveFileFormatValidator.GetValidPuids(archiveFileFormats.FileFormats);
            _regulationVersion = archiveFileFormats.RegulationVersion;

            foreach (IFileFormatInfo fileFormatInfo in fileFormatInfoSet)
            {
                if (fileFormatInfo == null)
                    continue;

                string fileName = Path.IsPathFullyQualified(fileFormatInfo.FileName)
                    ? Path.GetRelativePath(relativePathRoot, fileFormatInfo.FileName)
                    : fileFormatInfo.FileName;

                var documentFileListElement = new ListElement
                {
                    AbsoluteFilePath = fileFormatInfo.FileName,
                    FileName = fileName.Replace('\\', '/'),
                    FileExtension = fileFormatInfo.FileExtension,
                    FileFormatPuId = fileFormatInfo.Id,
                    FileFormatName = fileFormatInfo.Format,
                    FileFormatVersion = fileFormatInfo.Version,
                    FileMimeType = fileFormatInfo.MimeType,
                    FileScanError = fileFormatInfo.Errors,
                    FileSize = fileFormatInfo.ByteSize,
                    IsValidFormat = approvedPuidArchiveFormats.Contains(fileFormatInfo.Id) 
                        ? FormatAnalysisResultFileContent.FormatIsValidValue
                        : FormatAnalysisResultFileContent.FormatIsInvalidValue,
                };

                listElements.Add(documentFileListElement);

                FileTypeStatisticsElement existingStat;

                if ((existingStat = fileTypeStatisticsElements.Find(t => t.PuId.Equals(fileFormatInfo.Id))) != null)
                {
                    existingStat.Amount++;
                }
                else
                {
                    fileTypeStatisticsElements.Add(new FileTypeStatisticsElement
                    {
                        PuId = fileFormatInfo.Id,
                        FileFormatName = fileFormatInfo.Format,
                        FileFormatVersion = fileFormatInfo.Version,
                        Amount = 1,
                        IsValidFormat = documentFileListElement.IsValidFormat,
                    });
                }
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
                Map(m => m.AbsoluteFilePath).Name(FormatAnalysisResultFileContent.HeaderAbsoluteFilePath);
                Map(m => m.FileName).Name(FormatAnalysisResultFileContent.HeaderFileName);
                Map(m => m.FileExtension).Name(FormatAnalysisResultFileContent.HeaderFileExtension);
                Map(m => m.FileFormatPuId).Name(FormatAnalysisResultFileContent.HeaderFormatId);
                Map(m => m.FileFormatName).Name(FormatAnalysisResultFileContent.HeaderFormatName);
                Map(m => m.FileFormatVersion).Name(FormatAnalysisResultFileContent.HeaderFormatVersion);
                Map(m => m.FileMimeType).Name(FormatAnalysisResultFileContent.HeaderMimeType);
                Map(m => m.FileScanError).Name(FormatAnalysisResultFileContent.HeaderErrors);
                Map(m => m.FileSize).Name(FormatAnalysisResultFileContent.HeaderFileSize);
                Map(m => m.IsValidFormat).Name(_regulationVersion);
            }
        }

        private sealed class FileTypeStatisticsElementMap : ClassMap<FileTypeStatisticsElement>
        {
            public FileTypeStatisticsElementMap()
            {
                Map(m => m.PuId).Name(FormatAnalysisResultFileContent.StatisticsHeaderFormatId);
                Map(m => m.FileFormatName).Name(FormatAnalysisResultFileContent.StatisticsHeaderFileType);
                Map(m => m.FileFormatVersion).Name(FormatAnalysisResultFileContent.StatisticsHeaderFormatVersion);
                Map(m => m.Amount).Name(FormatAnalysisResultFileContent.StatisticsHeaderAmount);
                Map(m => m.IsValidFormat).Name(_regulationVersion);
            }
        }

        private class ListElement
        {
            public string AbsoluteFilePath { get; init; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public string FileFormatPuId { get; set; }
            public string FileFormatName { get; set; }
            public string FileFormatVersion { get; set; }
            public string FileMimeType { get; set; }
            public string FileScanError { get; set; }
            public string FileSize { get; set; }
            public string IsValidFormat { get; set; }
        }

        private class FileTypeStatisticsElement : IComparable
        {
            public string PuId { get; init; }
            public string FileFormatName { get; init; }
            public string FileFormatVersion { get; init; }
            public int Amount { get; set; }
            public string IsValidFormat { get; set; }

            public int CompareTo(object obj)
            {
                return obj is not FileTypeStatisticsElement other
                    ? 1
                    : string.Compare(FileFormatName, other.FileFormatName, StringComparison.InvariantCulture);
            }
        }
    }
}
