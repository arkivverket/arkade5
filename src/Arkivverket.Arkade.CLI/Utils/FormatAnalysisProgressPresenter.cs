using System;
using System.Reflection;
using System.Threading;
using Serilog;

namespace Arkivverket.Arkade.CLI.Utils
{
    internal class FormatAnalysisProgressPresenter
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static readonly Mutex Mutex = new(false, "ConsoleCursorPosition - 183f9057-3fd1-4d58-a69b-79ed60f43cfc");

        private long? _analysisTargetSize;
        private long _sizeOfAnalysedFiles;
        private decimal _previousProgressAsPercentage;
        private decimal _currentProgressAsPercentage;

        public FormatAnalysisProgressPresenter()
        {
            _analysisTargetSize = null;
            _sizeOfAnalysedFiles = 0;
            _previousProgressAsPercentage = 0;
        }

        public void SetTotalAmountOfFiles(long? analysisTargetSize)
        {
            _analysisTargetSize = analysisTargetSize;
        }

        public void UpdateAndDisplayProgress(long fileSize)
        {
            UpdateProgress(fileSize);
            DisplayProgress(GetProgressAsString());
            _previousProgressAsPercentage = _currentProgressAsPercentage;
        }

        private void UpdateProgress(long fileSize)
        {
            _sizeOfAnalysedFiles += fileSize;
        }

        private void DisplayProgress(string progress)
        {
            if (_currentProgressAsPercentage.ToString("P") == _previousProgressAsPercentage.ToString("P"))
                return;

            Mutex.WaitOne();
            Log.Information($"Performing file format analysis: {progress}");

            ResetCursorPosition();
            Mutex.ReleaseMutex();
        }

        public void DisplayFinished()
        {
            Mutex.WaitOne();
            Log.Information("Performing file format analysis: 100.00 %");
            Mutex.ReleaseMutex();
        }

        private string GetProgressAsString()
        {
            _currentProgressAsPercentage = _analysisTargetSize == null
                ? _previousProgressAsPercentage
                : _sizeOfAnalysedFiles/(decimal)_analysisTargetSize;

            return _currentProgressAsPercentage.ToString("P");
        }

        private static void ResetCursorPosition()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
