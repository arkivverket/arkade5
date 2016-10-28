using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Util;
using Serilog;
using Application = System.Windows.Application;

namespace Arkivverket.Arkade.UI
{
    public partial class App : Application
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<App>();

        public App()
        {
            // For some reason this will not work for exceptions thrown from inside the Views.
            AppDomain.CurrentDomain.UnhandledException += MyHandler;
        }

        public static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            ExceptionMessageBox.Show(e);
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