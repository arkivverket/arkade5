using System;
using System.Windows;
using System.Windows.Threading;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Util;
using Serilog;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Arkivverket.Arkade.UI
{
    public partial class App : Application
    {
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
            string errorMessage = $"An unhandled exception occurred: {e.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Log.Error(errorMessage, e);
        }

        /*
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            Exception e = args.Exception;
            //Console.WriteLine("MyHandler caught : " + e.Message);
            //Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);

            string errorMessage = $"An unhandled exception occurred: {e.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Log.Error(errorMessage, e);
        }
        */

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An unhandled exception occurred: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Log.Error(errorMessage, e);
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