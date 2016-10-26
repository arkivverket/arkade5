using System.Windows;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.UI
{
    public partial class App : Application
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<App>();

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogConfiguration.ConfigureSeriLog();
            Log.Information("Arkade " + ArkadeVersion.Version + " started");

            base.OnStartup(e);
            var bs = new Bootstrapper();
            bs.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Log.Information("Arkade " + ArkadeVersion.Version + " stopping");
        }
    }
}