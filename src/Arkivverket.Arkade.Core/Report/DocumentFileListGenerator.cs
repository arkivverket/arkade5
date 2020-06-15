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
            string filePath = Path.Combine(fileLocation, ArkadeConstants.DocumentFileListFileName);
            
            var fileFormatIdentifier = new SiegfriedFileFormatIdentifier();

            Dictionary<FileInfo, FileFormat> documentFilesAndFormats = fileFormatIdentifier.IdentifyFormat(archive.DocumentFiles.Values);

            string documentsDirectoryParentFullPath = archive.GetDocumentsDirectory().Parent?.FullName + Path.PathSeparator;

            var documentFileListElements = new List<DocumentFileListElement>();

            foreach (KeyValuePair<FileInfo, FileFormat> docFileAndFormat in documentFilesAndFormats)
            {
                FileFormat fileFormat = docFileAndFormat.Value;
                string relativeFilePath = docFileAndFormat.Key.FullName.Substring(documentsDirectoryParentFullPath.Length);
                documentFileListElements.Add(new DocumentFileListElement
                {
                    FileName = relativeFilePath,
                    FileFormatPuId = fileFormat.PuId,
                    FileFormatName = fileFormat.Name,
                    FileFormatVersion = fileFormat.Version
                });
            }

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(documentFileListElements);
            }
        }

        private class DocumentFileListElement
        {
            [Name(ArkadeConstants.DocumentFileListHeaders.FileName)]
            public string FileName { get; set; }
            [Name(ArkadeConstants.DocumentFileListHeaders.FormatId)]
            public string FileFormatPuId { get; set; }
            [Name(ArkadeConstants.DocumentFileListHeaders.FormatName)]
            public string FileFormatName { get; set; }
            [Name(ArkadeConstants.DocumentFileListHeaders.FormatVersion)]
            public string FileFormatVersion { get; set; }
        }
    }
}
