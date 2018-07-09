using System;
using System.Diagnostics;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.Properties;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.GUI
{
    public partial class App
    {
        private static ILogger _log;

        public App()
        {
            try
            {
                ArkadeProcessingArea.Establish(Settings.Default.ArkadeProcessingAreaLocation);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception while establishing arkade processing area: " + e.Message);
            }

            LogConfiguration.ConfigureSeriLog();
            _log = Log.ForContext<App>();
            // For some reason this will not work for exceptions thrown from inside the Views.
            AppDomain.CurrentDomain.UnhandledException += MyHandler;
        }

        public static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            new DetailedExceptionMessage(e).ShowMessageBox();
            _log.Error("Unexpected exception", e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _log.Information("Arkade " + ArkadeVersion.Current + " started");

            base.OnStartup(e);
            var bs = new Bootstrapper();
            bs.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _log.Information("Arkade " + ArkadeVersion.Current + " stopping");

            if (!ArkadeProcessingAreaLocationSetting.IsApplied())
                ArkadeProcessingArea.Destroy();

            else if (ArkadeInstance.IsOnlyInstance)
                ArkadeProcessingArea.CleanUp();

            base.OnExit(e);
        }
    }
}
