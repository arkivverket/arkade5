using System;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.CLI.Utils
{
    internal class FormatAnalysisProgressPresenter
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public long TotalAmountOfFiles { get; }
        public long FileCounter { get; set; }

        public FormatAnalysisProgressPresenter(long totalAmountOfFiles)
        {
            TotalAmountOfFiles = totalAmountOfFiles;
            FileCounter = 0;
        }

        public void DisplayProgress()
        {
            Log.Information($"Performing file format analysis: {FileCounter} of {TotalAmountOfFiles} files analysed");

            ResetCursorPosition();
        }

        public void DisplayFinished()
        {
            Log.Information($"Performing file format analysis: {TotalAmountOfFiles} files analysed");
        }

        private static void ResetCursorPosition()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
