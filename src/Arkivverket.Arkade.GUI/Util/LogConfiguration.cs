using System.Threading;
﻿using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Arkivverket.Arkade.GUI.Util
{
    public class LogConfiguration
    {
        public static void ConfigureSeriLog()
        {
            string systemLogFilePath = Path.Combine(
                ArkadeProcessingArea.LogsDirectory.ToString(),
                ArkadeConstants.SystemLogFileNameFormat
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.RollingFile(systemLogFilePath, outputTemplate: $"{Resources.GUI.SerilogFormatConfig}")
                .WriteTo.ColoredConsole(outputTemplate: $"{Resources.GUI.SerilogFormatConfig}")
                .CreateLogger();
        }
    }


    internal class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}