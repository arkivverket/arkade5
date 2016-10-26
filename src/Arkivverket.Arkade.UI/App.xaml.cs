using System;
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
            // Add the event handler for handling UI thread exceptions to the event.
            //System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            //System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ExceptionMessageBox.Show(e);
        }

        /*
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            Exception e = args.Exception;
            ExceptionMessageBox.Show(e);
         }
        */

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionMessageBox.Show(e.Exception);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LogConfiguration.ConfigureSeriLog();
            Log.Information("Arkade " + ArkadeVersion.Version + " started");

            //Application.Current.DispatcherUnhandledException +=
            //    new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);

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