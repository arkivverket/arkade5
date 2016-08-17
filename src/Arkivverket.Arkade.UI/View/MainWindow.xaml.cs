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

        public MainWindow()
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
