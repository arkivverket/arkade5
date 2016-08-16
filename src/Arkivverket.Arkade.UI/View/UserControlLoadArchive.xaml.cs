using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Util;
using Autofac;

namespace Arkivverket.Arkade.UI.View
{
    /// <summary>
    /// Interaction logic for UserControlLoadArchive.xaml
    /// </summary>
    public partial class UserControlLoadArchive : UserControl
    {
        public UserControlLoadArchive()
        {
            InitializeComponent();
        }


        private void btnSelectArchive_Click(object sender, RoutedEventArgs e)
        {
            txtArchiveFileName.Text = new FileFolderDialogs().ChooseFile(Properties.Resources.FileSelectionWindowNameArchive, "*", "");
        }

        private void btnSelectMetadata_Click(object sender, RoutedEventArgs e)
        {
            txtMetadataFileName.Text = new FileFolderDialogs().ChooseFile(Properties.Resources.FileSelectionWindowNameArchive, "*", "");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = "Laster inn arkiv....";

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            var container = builder.Build();
            using (container.BeginLifetimeScope())
            {
                ArchiveExtractionReader archiveExtractionReader = container.Resolve<ArchiveExtractionReader>();

                ArchiveExtraction archiveExtraction = archiveExtractionReader.ReadFromFile(txtArchiveFileName.Text, txtMetadataFileName.Text);

                txtResult.Text = $"Uuid: {archiveExtraction.Uuid}\n" +
                    $"WorkingDirectory: {archiveExtraction.WorkingDirectory}\n" +
                    $"ArchiveType: {archiveExtraction.ArchiveType}";
            }
        }



    }
}
