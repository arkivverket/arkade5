using System.Threading;
using Arkivverket.Arkade.Util;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Arkivverket.Arkade.UI.Util
{
    public class LogConfiguration
    {
        public static void ConfigureSeriLog()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(ApplicationSettings.ApplicationDataDirectory() + "\\arkade-log-{Date}.txt", outputTemplate: $"{Resources.UI.SerilogFormatConfig}")
                .WriteTo.ColoredConsole(outputTemplate: $"{Resources.UI.SerilogFormatConfig}")
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