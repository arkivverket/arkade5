using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Threading;

namespace Arkivverket.Arkade.UI.Util
{
    public class LogConfiguration
    {

        public static void ConfigureSeriLog()
        {
            // Init logging
            Log.Logger = new LoggerConfiguration()
                                .Enrich.With(new ThreadIdEnricher())
                                .WriteTo.ColoredConsole(outputTemplate: $"{Properties.Resources.SerilogFormatConfig}")
                                .CreateLogger();
        }

    }


    class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }


}
