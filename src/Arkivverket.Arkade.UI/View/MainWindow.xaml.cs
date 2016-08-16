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

            // Init logging
            LogConfiguration.ConfigureSeriLog();

        }
    }
}
