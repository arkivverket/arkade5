using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.UI.ViewModels;
using Arkivverket.Arkade.Util;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class MainWindow : Window
    {
        public static bool TestsIsRunningOrHasRun { get; set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Title = UI.Resources.UI.General_WindowTitle;
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
                DialogResult dialogResult = MessageBox.Show(UI.Resources.UI.UnsavedTestResultsOnExitWarning,
                    "NB!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if(dialogResult == System.Windows.Forms.DialogResult.No)
                        e.Cancel = true;
            }
        }
    }
}