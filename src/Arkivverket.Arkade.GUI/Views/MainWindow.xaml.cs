using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.ViewModels;
using Arkivverket.Arkade.Core.Util;
using MessageBox = System.Windows.Forms.MessageBox;

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
                DialogResult dialogResult = MessageBox.Show(GUI.Resources.GUI.UnsavedTestResultsOnExitWarning,
                    "NB!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if(dialogResult == System.Windows.Forms.DialogResult.No)
                        e.Cancel = true;
            }
        }
    }
}