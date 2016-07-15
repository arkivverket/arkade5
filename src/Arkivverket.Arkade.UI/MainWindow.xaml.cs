using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Core;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Serilog;
using System.IO;
using System.Threading;

namespace Arkivverket.Arkade.UI
{
    public partial class MainWindow : Window
    {

        // Init Async structures
        public Progress<BigSlowStatus> progressIndicator;
        public CancellationTokenSource cts = new CancellationTokenSource();


        public MainWindow()
        {
            InitializeComponent();
            Title = Properties.Resources.General_WindowTitle;
            progressBar.Value = 0;

            // Init logging
            LogConfiguration.ConfigureSeriLog();

        }



        private void loadArchive_Click(object sender, RoutedEventArgs e)
        {
            string filename = new FileFolderDialogs().ChooseFile(Properties.Resources.FileSelectionWindowNameArchive,
                                                                 Properties.Resources.FileSelectionDefaultTar, 
                                                                 Properties.Resources.FileSelectionFilterTar);

            textBoxLogMessages.AppendText(filename);
        }

        private async void testButton_Click(object sender, RoutedEventArgs e)
        {


            progressIndicator = new Progress<BigSlowStatus>(ReportProgress);

            try
            {
                int counts = await new BigSlowAsync().DoSomethingRatherSlow(progressIndicator, cts.Token);
            }
            catch
            {
                LogMessage("Exception in the Async");
                Log.Fatal("Exception in the Async");
            }

        }

        public void ReportProgress(BigSlowStatus status)
        {
            progressBar.Value = status.PctDone;
            LogMessage(status.MyMessageToTheWorld);
            LogMessage($"Done: {status.IsDone}");
            Log.Information($"Big and slow {progressBar.Value}");
        }

        private void squirrelButton_Click(object sender, RoutedEventArgs e)
        {
           LogMessage("Squirrel!!!");
        }

        public void LogMessage(string log)
        {
            textBoxLogMessages.AppendText($"{log}\n");
            textBoxLogMessages.ScrollToEnd();
        }

        private void quit_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }
}
