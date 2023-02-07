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

        public long? TotalAmountOfFiles { get; private set; }
        public long FileCounter { get; set; }

        public FormatAnalysisProgressPresenter()
        {
            TotalAmountOfFiles = null;
            FileCounter = 0;
        }

        public void SetTotalAmountOfFiles(long totalAmountOfFiles)
        {
            TotalAmountOfFiles = totalAmountOfFiles;
        }

        public void DisplayProgress()
        {
            Mutex.WaitOne();
            Log.Information($"Performing file format analysis: {FileCounter} of {TotalAmountOfFiles?.ToString() ?? "(...)"} files analysed");

            ResetCursorPosition();
            Mutex.ReleaseMutex();
        }

        public void DisplayFinished()
        {
            Mutex.WaitOne();
            Log.Information("Performing file format analysis:                                                                       ");
            ResetCursorPosition();
            Log.Information($"Performing file format analysis: {TotalAmountOfFiles} files analysed");
            Mutex.ReleaseMutex();
        }

        private static void ResetCursorPosition()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
