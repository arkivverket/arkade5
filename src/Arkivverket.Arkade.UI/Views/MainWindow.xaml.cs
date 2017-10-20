using System;
using System.Windows;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.UI.ViewModels;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Title = string.Format(UI.Resources.UI.General_WindowTitle, ArkadeVersion.Version);
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
    }
}