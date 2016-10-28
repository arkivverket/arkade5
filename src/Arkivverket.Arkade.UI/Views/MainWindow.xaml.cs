using System;
using System.Windows;
using Arkivverket.Arkade.UI.Util;
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
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
                throw;
            }
        }
    }
}