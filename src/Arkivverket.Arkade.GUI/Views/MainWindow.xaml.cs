using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shell;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.Models;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    public partial class MainWindow : Window
    {
        public static readonly BackgroundWorker ProgressBarWorker = new BackgroundWorker();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Title = Languages.GUI.General_WindowTitle;
                Loaded += (sender, e) =>
                {
                    if (!ArkadeProcessingAreaLocationSetting.IsValid())
                        ((MainWindowViewModel) DataContext).ShowInvalidProcessingAreaLocationDialogCommand.Execute();
                };
                ProgressBarWorker.WorkerReportsProgress = true;
                ProgressBarWorker.ProgressChanged += OnProgressChanged;
            }
            catch (Exception e)
            {
                new DetailedExceptionMessage(e).ShowMessageBox();
                throw;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (ArkadeProcessingState.TestingIsStarted && !ArkadeProcessingState.PackingIsFinished)
            {
                MessageBoxResult dialogResult = MessageBox.Show(Languages.GUI.UnsavedTestResultsOnExitWarning,
                    "NB!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if(dialogResult == MessageBoxResult.No)
                        e.Cancel = true;
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState?.ToString() == "reset")
                Application.Current.Dispatcher.Invoke(() => (
                    taskbarItemInfo.ProgressState = TaskbarItemProgressState.None
                ));
            else if (e.ProgressPercentage == 0)
                Application.Current.Dispatcher.Invoke(() => (
                    taskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate
                ));
            else if (e.ProgressPercentage == 100)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    taskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                    taskbarItemInfo.ProgressValue = 1.0;
                });
        }
    }
}