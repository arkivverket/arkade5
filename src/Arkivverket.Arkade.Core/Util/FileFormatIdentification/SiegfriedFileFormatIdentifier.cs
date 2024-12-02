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

            IEnumerable<IFileFormatInfo> siegfriedFileInfoObjects = AnalyseFiles(target, scanMode);

            _statusEventHandler.RaiseEventFormatAnalysisFinished();

            return siegfriedFileInfoObjects;
        }

        private IEnumerable<IFileFormatInfo> AnalyseFiles(string target, FileFormatScanMode scanMode)
        {
            Process siegfriedProcess = _processRunner.SetupSiegfriedProcess(scanMode, target);

            IEnumerable<string> siegfriedResult = _processRunner.Run(siegfriedProcess);

            HashSet<IFileFormatInfo> siegfriedFileInfoObjects = ExternalProcessManager.TryClose(siegfriedProcess)
                ? GetFileFormatInfoObjects(siegfriedResult).ToHashSet()
                : throw new SiegfriedFileFormatIdentifierException("Process does not exist, or has been terminated");

            if (scanMode == FileFormatScanMode.Directory)
                AppendArchiveFileContents(siegfriedFileInfoObjects);

            return siegfriedFileInfoObjects;
        }

        private void AppendArchiveFileContents(ISet<IFileFormatInfo> siegfriedFileInfoObjects)
        {
            List<string> archiveFilePaths = siegfriedFileInfoObjects.Where(
                s => _supportedArchiveFilePronomCodes.Contains(s.Id)
            ).Select(s => s.FileName).ToList();

            IEnumerable<IFileFormatInfo> fileFormatInfos = Task.WhenAll(
                archiveFilePaths.Select(p => AnalyseFilesAsync(p, FileFormatScanMode.Archive))
            ).Result.SelectMany(a => a);
                
            foreach (IFileFormatInfo fileFormatInfo in fileFormatInfos)
                siegfriedFileInfoObjects.Add(fileFormatInfo);
        }

        private async Task<IEnumerable<IFileFormatInfo>> AnalyseFilesAsync(string target, FileFormatScanMode scanMode)
        {
            return await Task.Run(() => AnalyseFiles(target, scanMode));
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
                    SiardMessages.InlinedLobContentHasUnsupportedEncoding, "N/A", "N/A", "N/A", "N/A");

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
