using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace Arkivverket.Arkade.Core.Report
{
    public static class DocumentFileListGenerator
    {
        public static void Generate(string fileLocation, Archive archive)
        {
            DirectoryInfo documentsDirectory = archive.GetDocumentsDirectory();

            IEnumerable<SiegfriedFileInfo> siegfriedFileInfoSet = GetFormatInfoAllFiles(documentsDirectory);

            string filePathStart = archive.WorkingDirectory.Content().DirectoryInfo().Parent?.FullName +
                                   Path.DirectorySeparatorChar;

            IEnumerable<ListElement> listElements = GetListElements(siegfriedFileInfoSet, filePathStart);

            WriteFileList(listElements, fileLocation);
        }

        private static IEnumerable<SiegfriedFileInfo> GetFormatInfoAllFiles(DirectoryInfo directory)
        {
            var fileFormatIdentifier = new SiegfriedFileFormatIdentifier();

            return fileFormatIdentifier.IdentifyFormat(directory);
        }

        private static IEnumerable<ListElement> GetListElements(IEnumerable<SiegfriedFileInfo> siegfriedFileInfoSet,
            string filePathStart)
        {
            var listElements = new List<ListElement>();

            foreach (SiegfriedFileInfo siegfriedFileInfo in siegfriedFileInfoSet)
            {
                var documentFileListElement = new ListElement
                {
                    FileName = siegfriedFileInfo.FileName.Substring(filePathStart.Length),
                    FileFormatPuId = siegfriedFileInfo.Id,
                    FileFormatName = siegfriedFileInfo.Format,
                    FileFormatVersion = siegfriedFileInfo.Version,
                    FileScanError = siegfriedFileInfo.Errors
                };

                listElements.Add(documentFileListElement);
            }

            return listElements;
        }

        private static void WriteFileList(IEnumerable<ListElement> listElements, string fileLocation)
        {
            string fullFileName = Path.Combine(fileLocation, ArkadeConstants.DocumentFileListFileName);

            using (var writer = new StreamWriter(fullFileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listElements);
            }
        }

        private class ListElement
        {
            [Name(ArkadeConstants.DocumentFileListHeaders.FileName)]
            public string FileName { get; set; }

            [Name(ArkadeConstants.DocumentFileListHeaders.FormatId)]
            public string FileFormatPuId { get; set; }

            [Name(ArkadeConstants.DocumentFileListHeaders.FormatName)]
            public string FileFormatName { get; set; }

            [Name(ArkadeConstants.DocumentFileListHeaders.FormatVersion)]
            public string FileFormatVersion { get; set; }

            [Name(ArkadeConstants.DocumentFileListHeaders.FileScanError)]
            public string FileScanError { get; set; }
        }
    }
}
