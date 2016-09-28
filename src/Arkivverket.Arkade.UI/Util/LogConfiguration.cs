using System.Threading;
using Arkivverket.Arkade.UI.Properties;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Arkivverket.Arkade.UI.Util
{
    public class LogConfiguration
    {
        public static void ConfigureSeriLog()
        {
            // Init logging
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
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