using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        private static SiegfriedProcessRunner _processRunner;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly IFileSystemInfoSizeCalculator _fileSystemInfoSizeCalculator;

        private readonly List<string> _supportedArchiveFilePronomCodes = new()
        {
            "x-fmt/263", //.zip
            "x-fmt/265", //.tar
            "fmt/289", "fmt/1355", "fmt/1281", //.warc
            "fmt/161", "fmt/995", "fmt/1196", "fmt/1777" // .siard
        };

        public SiegfriedFileFormatIdentifier(SiegfriedProcessRunner siegfriedProcessRunner,
            IStatusEventHandler statusEventHandler, IFileSystemInfoSizeCalculator fileSystemInfoSizeCalculator)
        {
            _processRunner = siegfriedProcessRunner;
            _statusEventHandler = statusEventHandler;
            _fileSystemInfoSizeCalculator = fileSystemInfoSizeCalculator;
        }

        public void BroadCastStarted()
        {
            _statusEventHandler.RaiseEventFormatAnalysisStarted();
        }

        public void BroadCastFinished()
        {
            _statusEventHandler.RaiseEventFormatAnalysisFinished();
        }

        public IEnumerable<IFileFormatInfo> IdentifyFormats(string target, FileFormatScanMode scanMode)
        {
            _statusEventHandler.RaiseEventFormatAnalysisStarted();

            _fileSystemInfoSizeCalculator.CalculateSize(scanMode, target);

            IEnumerable<IFileFormatInfo> siegfriedFileInfoObjects;
            
            if (scanMode == FileFormatScanMode.Archive)
            {
                Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, target);

                IEnumerable<string> siegfriedResults = _processRunner.RunOnArchive(siegfriedProcess);

                ExternalProcessManager.Close(siegfriedProcess); // TODO: Call after AnalyseFiles also?

                siegfriedFileInfoObjects = GetFileFormatInfoObjects(siegfriedResults);
            }
            else
            {
                siegfriedFileInfoObjects = AnalyseFiles(target, scanMode);
            }

            _statusEventHandler.RaiseEventFormatAnalysisFinished();

            return siegfriedFileInfoObjects;
        }

        private IEnumerable<IFileFormatInfo> AnalyseFiles(string target, FileFormatScanMode scanMode)
        {
            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, target);

            IEnumerable<string> siegfriedResult = _processRunner.Run(siegfriedProcess);

            List<IFileFormatInfo> siegfriedFileInfoObjects = ExternalProcessManager.TryClose(siegfriedProcess)
                ? GetFileFormatInfoObjects(siegfriedResult).ToList()
                : throw new SiegfriedFileFormatIdentifierException("Process does not exist, or has been terminated");

            if (!SiegfriedFileInfoObjectsContainsArchiveFiles(ref siegfriedFileInfoObjects, scanMode))
                return siegfriedFileInfoObjects;

            List<IFileFormatInfo> archiveFilePaths = siegfriedFileInfoObjects.Where(s =>
                _supportedArchiveFilePronomCodes.Contains(s.Id)).ToList();

            IEnumerable<Task<IEnumerable<IFileFormatInfo>>> archiveFormatAnalysisTasks = archiveFilePaths
                .Select(f => AnalyseFilesAsync(f.FileName, FileFormatScanMode.Archive));

            siegfriedFileInfoObjects.AddRange(Task.WhenAll(archiveFormatAnalysisTasks).Result.SelectMany(a => a));

            return siegfriedFileInfoObjects;
        }

        private async Task<IEnumerable<IFileFormatInfo>> AnalyseFilesAsync(string target, FileFormatScanMode scanMode)
        {
            return await Task.Run(() => AnalyseFiles(target, scanMode));
        }

        private bool SiegfriedFileInfoObjectsContainsArchiveFiles(ref List<IFileFormatInfo> fileFormatInfoObjects,
            FileFormatScanMode scanMode)
        {
            if (scanMode == FileFormatScanMode.Archive)
            {
                // Skip first element when .zip (or similar) have been analysed, as this element is the .zip file itself
                fileFormatInfoObjects = fileFormatInfoObjects.Skip(1).ToList();
            }
            return fileFormatInfoObjects.Any(f => _supportedArchiveFilePronomCodes.Contains(f.Id));
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

            return GetFileFormatInfoObject(siegfriedResult);
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

                return GetFileFormatInfoObject(siegfriedResult);
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

        private static IEnumerable<IFileFormatInfo> GetFileFormatInfoObjects(IEnumerable<string> formatInfoSet)
        {
            return formatInfoSet.Skip(1).Where(f => f != null).Select(GetFileFormatInfoObject);
        }

        private static IFileFormatInfo GetFileFormatInfoObject(string siegfriedFormatResult)
        {
            return SiegfriedFileInfo.CreateFromString(siegfriedFormatResult);
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
