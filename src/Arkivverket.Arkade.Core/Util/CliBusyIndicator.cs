using System;
using System.ComponentModel;
using System.Threading;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    internal class CliBusyIndicator : IBusyIndicator
    {
        private static readonly BackgroundWorker BusyIndicator = InitializeBackgroundWorker();

        private static readonly Mutex Mutex = new(false, "ConsoleCursorPosition - 183f9057-3fd1-4d58-a69b-79ed60f43cfc");

        private static string _busyIndicatorString = @"|~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ |";
        private static int _busyIndicatorCursorLeftPosition;
        private static int _busyIndicatorCursorTopPosition;
        private const int MillisecondsPerTick = 75;

        public void Start()
        {
            Mutex.WaitOne();
            _busyIndicatorCursorLeftPosition = Console.CursorLeft;
            _busyIndicatorCursorTopPosition = Console.CursorTop;
            Mutex.ReleaseMutex();

            if (!BusyIndicator.IsBusy)
                BusyIndicator.RunWorkerAsync();
            else
                Log.Error("BusyIndicator is already running, cannot start again.");
        }

        public void Stop()
        {
            BusyIndicator.CancelAsync();
            while (BusyIndicator.IsBusy)
                Thread.Sleep(50);

            ClearBusyIndicatorLine();
        }

        private static void ClearBusyIndicatorLine()
        {
            Mutex.WaitOne();
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            Console.SetCursorPosition(_busyIndicatorCursorLeftPosition, _busyIndicatorCursorTopPosition);
            Console.WriteLine(@"|~~~ ~~~ ~~~ ~~~ ~~~ ~~~ Done! ~~~ ~~~ ~~~ ~~~ ~~~ ~~~ ~~|");
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Mutex.ReleaseMutex();
        }

        private static BackgroundWorker InitializeBackgroundWorker()
        {
            BackgroundWorker backgroundWorker = new()
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true,
            };

            backgroundWorker.DoWork += delegate
            {
                while (!backgroundWorker.CancellationPending)
                {
                    Mutex.WaitOne();
                    int cursorLeft = Console.CursorLeft;
                    int cursorTop = Console.CursorTop;
                    Console.SetCursorPosition(_busyIndicatorCursorLeftPosition, _busyIndicatorCursorTopPosition);
                    Console.WriteLine(_busyIndicatorString);
                    if (cursorTop != _busyIndicatorCursorTopPosition)
                        Console.SetCursorPosition(cursorLeft, cursorTop);

                    Mutex.ReleaseMutex();

                    UpdateBusyIndicatorString();

                    Thread.Sleep(MillisecondsPerTick);
                }
            };

            return backgroundWorker;
        }

        private static void UpdateBusyIndicatorString()
        {
            char shiftChar = _busyIndicatorString[^2];
            _busyIndicatorString = _busyIndicatorString.Remove(_busyIndicatorString.Length - 2, 1);
            _busyIndicatorString = _busyIndicatorString.Insert(1, shiftChar.ToString());
        }
    }
}
