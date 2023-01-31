using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using CsvHelper;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        private static SiegfriedProcessRunner _processRunner;
        private readonly IStatusEventHandler _statusEventHandler;

        private readonly List<string> _supportedZipFormatExtension = new()
        {
            ".zip", ".tar", ".gz", ".arc", ".warc"
        };

        public SiegfriedFileFormatIdentifier(SiegfriedProcessRunner siegfriedProcessRunner,
            IStatusEventHandler statusEventHandler)
        {
            _processRunner = siegfriedProcessRunner;
            _statusEventHandler = statusEventHandler;
        }
        
        public IEnumerable<IFileFormatInfo> IdentifyFormats(string target, FileFormatScanMode scanMode)
        {
            _statusEventHandler.RaiseEventFormatAnalysisStarted(0); // reset file counter

            long numberOfFilesToAnalyse = CalculateNumberOfFilesToAnalyse(scanMode, target);

            _statusEventHandler.RaiseEventFormatAnalysisStarted(numberOfFilesToAnalyse);

            IEnumerable<IFileFormatInfo> siegfriedFileInfoObjects = AnalyseFiles(target, scanMode, numberOfFilesToAnalyse);

            _statusEventHandler.RaiseEventFormatAnalysisFinished();

            return siegfriedFileInfoObjects;
        }

        private IEnumerable<IFileFormatInfo> AnalyseFiles(string target, FileFormatScanMode scanMode, long numberOfFilesToAnalyse)
        {
            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, target);

            IEnumerable<string> siegfriedResult = _processRunner.Run(siegfriedProcess, numberOfFilesToAnalyse);

            int siegfriedCloseStatus = ExternalProcessManager.Close(siegfriedProcess.Id);

            List<IFileFormatInfo> siegfriedFileInfoObjects = siegfriedCloseStatus switch
            {
                -1 => throw new SiegfriedFileFormatIdentifierException("Process does not exist"),
                1 => throw new SiegfriedFileFormatIdentifierException("Process was terminated"),
                _ => GetSiegfriedFileInfoObjects(siegfriedResult).ToList(),
            };

            if (!SiegfriedFileInfoObjectsContainsArchiveFiles(siegfriedFileInfoObjects, scanMode))
                return siegfriedFileInfoObjects;

            List<IFileFormatInfo> archiveFilePaths = siegfriedFileInfoObjects.Where(s =>
                _supportedZipFormatExtension.Contains(s.FileExtension)).ToList();

            IEnumerable<Task<IEnumerable<IFileFormatInfo>>> archiveFormatAnalysisTasks = archiveFilePaths
                .Select(f => AnalyseFilesAsync(f.FileName, FileFormatScanMode.Archive, numberOfFilesToAnalyse));

            siegfriedFileInfoObjects.AddRange(Task.WhenAll(archiveFormatAnalysisTasks).Result.SelectMany(a => a));

            return siegfriedFileInfoObjects;
        }

        private async Task<IEnumerable<IFileFormatInfo>> AnalyseFilesAsync(string target, FileFormatScanMode scanMode,
            long numberOfFilesToAnalyse)
        {
            return await Task.Run(() => AnalyseFiles(target, scanMode, numberOfFilesToAnalyse));
        }
        private bool SiegfriedFileInfoObjectsContainsArchiveFiles(
            IEnumerable<IFileFormatInfo> fileFormatInfoObjects, FileFormatScanMode scanMode)
        {
            return (scanMode == FileFormatScanMode.Archive
                ? fileFormatInfoObjects.Skip(1) // Skip first element when .zip (or similar) have been analysed, as this element is the .zip file itself
                : fileFormatInfoObjects).Any(f => _supportedZipFormatExtension.Contains(f.FileExtension));
        }

        public IEnumerable<IFileFormatInfo> IdentifyFormats(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> filePathsAndByteContent)
        {
            List<Task<IFileFormatInfo>> fileFormatTasks = new();

            foreach (var filePathAndByteContent in filePathsAndByteContent)
            {
                fileFormatTasks.Add(IdentifyFormatAsync(filePathAndByteContent));
            }

            return Task.WhenAll(fileFormatTasks).Result;
        }

        private async Task<IFileFormatInfo> IdentifyFormatAsync(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            return await Task.Run(() => IdentifyFormat(filePathAndByteContent));
        }

        public IFileFormatInfo IdentifyFormat(FileInfo file)
        {
            const FileFormatScanMode scanMode = FileFormatScanMode.File;

            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, file.FullName);

            string siegfriedResult = _processRunner.RunOnFile(siegfriedProcess);

            ExternalProcessManager.Close(siegfriedProcess);

            return GetSiegfriedFileInfoObject(siegfriedResult);
        }

        public IFileFormatInfo IdentifyFormat(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            if (filePathAndByteContent.Value == null)
                return FileFormatInfoFactory.Create(filePathAndByteContent.Key,
                    filePathAndByteContent.Value?.Count().ToString() ?? "N/A",
                    Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding, "N/A", "N/A", "N/A", "N/A");

            const FileFormatScanMode scanMode = FileFormatScanMode.Stream;

            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, string.Empty);


            try
            {
                string siegfriedResult = _processRunner.RunOnByteArray(siegfriedProcess, filePathAndByteContent);

                return GetSiegfriedFileInfoObject(siegfriedResult);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                Log.Error($"Was not able to analyse {filePathAndByteContent.Key} - please see logfile for details.");
                return FileFormatInfoFactory.Create(filePathAndByteContent.Key, 
                    filePathAndByteContent.Value?.Count().ToString() ?? "N/A",
                    SiardMessages.ErrorMessage, "N/A", "N/A", "N/A", "N/A");
            }
            finally
            {
                ExternalProcessManager.Close(siegfriedProcess);
            }
        }

        private static IEnumerable<IFileFormatInfo> GetSiegfriedFileInfoObjects(IEnumerable<string> formatInfoSet)
        {
            return formatInfoSet.Skip(1).Where(f => f != null).Select(GetSiegfriedFileInfoObject);
        }

        private static SiegfriedFileInfo GetSiegfriedFileInfoObject(string siegfriedFormatResult)
        {
            if (siegfriedFormatResult == null)
                return null;

            using (var stringReader = new StringReader(siegfriedFormatResult))
            using (var csvParser = new CsvParser(stringReader, CultureInfo.InvariantCulture))
            {
                csvParser.Read();

                return new SiegfriedFileInfo
                (
                    fileName: csvParser.Record[0],
                    byteSize: csvParser.Record[1],
                    errors: csvParser.Record[3],
                    id: csvParser.Record[5],
                    format: csvParser.Record[6],
                    version: csvParser.Record[7],
                    mimeType: csvParser.Record[8]
                );
            }
        }

        private long CalculateNumberOfFilesToAnalyse(FileFormatScanMode scanMode, string target)
        {
            if (scanMode is FileFormatScanMode.Directory)
                return CalculateNumberOfFilesToAnalyse(new DirectoryInfo(target));

            if (scanMode is FileFormatScanMode.Archive)
                return CalculateNumberOfFilesToAnalyse(target);

            return 1;
        }

        private long CalculateNumberOfFilesToAnalyse(DirectoryInfo directory)
        {
            IEnumerable<FileInfo> allFiles = directory.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            });

            long numberOfFilesToAnalyse = allFiles.LongCount();

            return numberOfFilesToAnalyse + allFiles
                .Where(f => _supportedZipFormatExtension.Contains(f.Extension))
                .Select(f => CalculateNumberOfFilesToAnalyse(f.FullName)).Sum();
        }

        private long CalculateNumberOfFilesToAnalyse(string pathToArchiveFile)
        {
            using var zipArchive = new ZipArchive(File.OpenRead(pathToArchiveFile));
            return zipArchive.Entries.LongCount(e => e.Name != string.Empty);
        }
    }

    public class SiegfriedFileFormatIdentifierException : Exception
    {
        public SiegfriedFileFormatIdentifierException()
        {
        }

        public SiegfriedFileFormatIdentifierException(string message)
            : base(message)
        {
        }

        public SiegfriedFileFormatIdentifierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
