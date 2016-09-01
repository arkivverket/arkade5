using System.Windows;
using Arkivverket.Arkade.UI.Util;

namespace Arkivverket.Arkade.UI.View
{
    public partial class MainWindowOld : Window
    {

        public MainWindowOld()
        {
            InitializeComponent();
            Title = Properties.Resources.General_WindowTitle;

            // Init logging
            LogConfiguration.ConfigureSeriLog();

        }

        private void menuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
