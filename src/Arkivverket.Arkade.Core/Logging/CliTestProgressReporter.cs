﻿using System;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Logging
{
    internal class CliTestProgressReporter : ITestProgressReporter
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IStatusEventHandler _statusEventHandler;
        private readonly IBusyIndicator _busyIndicator;

        private static ArchiveType _archiveType;
        private int _progressPercentageConsoleCursorLeftLocation;
        private int _progressPercentageConsoleCursorTopLocation;
        private int _previousTestProgressValue;

        public CliTestProgressReporter(IStatusEventHandler statusEventHandler, IBusyIndicator busyIndicator)
        {
            _statusEventHandler = statusEventHandler;
            _busyIndicator = busyIndicator;
        }

        public void Begin(ArchiveType archiveType)
        {
            Console.CursorVisible = false;

            _archiveType = archiveType;

            var message = $"Running {archiveType}-validation: ";
            Log.Information(message);

            if (archiveType is ArchiveType.Siard)
            {
                _busyIndicator.Start();
                return;
            }
            
            _progressPercentageConsoleCursorLeftLocation = message.Length + "[Information] ".Length;
            _progressPercentageConsoleCursorTopLocation = Console.CursorTop - 1;
            SetConsoleCursorToTestProgressWriteLocation();

            _statusEventHandler.RaiseEventTestProgressUpdated("0 %");
            _previousTestProgressValue = 0;
        }

        public void ReportTestProgress(int testProgressValue)
        {
            if (_archiveType == ArchiveType.Siard)
                return;

            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            SetConsoleCursorToTestProgressWriteLocation();

            if (_previousTestProgressValue != testProgressValue)
            {
                _statusEventHandler.RaiseEventTestProgressUpdated($"{testProgressValue} %");
                _previousTestProgressValue = testProgressValue;
            }

            ResetCursorPositionToPreviousWriteLocation(cursorLeft, cursorTop);
        }

        public void Finish()
        {
            if (_archiveType is ArchiveType.Siard)
            {
                _busyIndicator.Stop();
                return;
            }

            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            SetConsoleCursorToTestProgressWriteLocation();

            _statusEventHandler.RaiseEventTestProgressUpdated("100 %");

            ResetCursorPositionToPreviousWriteLocation(cursorLeft, cursorTop);
        }

        private void SetConsoleCursorToTestProgressWriteLocation()
        {
            Console.SetCursorPosition(_progressPercentageConsoleCursorLeftLocation, _progressPercentageConsoleCursorTopLocation);
        }

        private static void ResetCursorPositionToPreviousWriteLocation(int cursorLeft, int cursorTop)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}