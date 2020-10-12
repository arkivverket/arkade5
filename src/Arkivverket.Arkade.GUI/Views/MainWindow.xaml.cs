using System;
using System.ComponentModel;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    public partial class MainWindow : Window
    {
        public static bool TestsIsRunningOrHasRun { get; set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Title = GUI.Resources.GUI.General_WindowTitle;
                Loaded += (sender, e) =>
                {
                    if (!ArkadeProcessingAreaLocationSetting.IsValid())
                        ((MainWindowViewModel) DataContext).ShowInvalidProcessingAreaLocationDialogCommand.Execute();
                };
            }
            catch (Exception e)
            {
                new DetailedExceptionMessage(e).ShowMessageBox();
                throw;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (TestsIsRunningOrHasRun && !InformationPackageCreator.HasRun)
            {
                MessageBoxResult dialogResult = MessageBox.Show(GUI.Resources.GUI.UnsavedTestResultsOnExitWarning,
                    "NB!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if(dialogResult == MessageBoxResult.No)
                        e.Cancel = true;
            }
        }
    }
}