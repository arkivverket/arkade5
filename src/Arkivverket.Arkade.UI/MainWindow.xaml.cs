using Arkivverket.Arkade.UI.Util;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Arkivverket.Arkade.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = Properties.Resources.General_WindowTitle;

        }

        private void loadArchive_Click(object sender, RoutedEventArgs e)
        {
            string filename = new FileFolderDialogs().ChooseFile(Properties.Resources.FileSelectionWindowNameArchive,
                                                                 Properties.Resources.FileSelectionDefaultTar, 
                                                                 Properties.Resources.FileSelectionFilterTar);

            textBoxLogMessages.AppendText(filename);
        }
    }
}
