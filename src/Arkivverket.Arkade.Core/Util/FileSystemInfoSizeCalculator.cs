using System.ComponentModel;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;

namespace Arkivverket.Arkade.Core.Util
{
    public class FileSystemInfoSizeCalculator : IFileSystemInfoSizeCalculator
    {
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly BackgroundWorker _fileCounter;

        public bool IsBusy => _fileCounter.IsBusy;

        public FileSystemInfoSizeCalculator(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
            _fileCounter = new BackgroundWorker();
            _fileCounter.DoWork += FileCounterOnDoWork;
        }

        public void CalculateSize(FileFormatScanMode scanMode, string target)
        {
            _fileCounter.RunWorkerAsync(new { ScanMode = scanMode, Target = target });
        }

        private void FileCounterOnDoWork(object sender, DoWorkEventArgs e)
        {
            var scanArgs = e.Argument as dynamic;
            long targetFileSize = GetTargetFileSize(scanArgs.ScanMode, scanArgs.Target);
            _statusEventHandler.RaiseEventTargetSizeCalculatorFinished(targetFileSize);
        }

        private static long GetTargetFileSize(FileFormatScanMode scanMode, string target)
        {
            return scanMode switch
            {
                FileFormatScanMode.Directory => CalculateSizeOfDirectory(new DirectoryInfo(target)),
                _ => new FileInfo(target).Length
            };
        }

        private static long CalculateSizeOfDirectory(DirectoryInfo directory)
        {
            return directory.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
            }).Select(f => f.Length).Sum();
        }
    }
}
