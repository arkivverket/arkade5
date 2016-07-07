using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Arkivverket.Arkade.App_Config
{
    public class LoggerConfig
    {
        private void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                             .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} {SourceContext} [{Level}] {Message}{NewLine}{Exception}")
                             .CreateLogger();
        }

    }
}
