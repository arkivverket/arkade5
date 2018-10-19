using System;
using Arkivverket.Arkade.GUI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Arkivverket.Arkade.Core.Metadata;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for CreatePackage.xaml
    /// </summary>
    public partial class CreatePackage : UserControl
    {
        public CreatePackage()
        {
            InitializeComponent();
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)sender;
            Popup popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
            System.Windows.Controls.Calendar cal = (System.Windows.Controls.Calendar)popup.Child;
            cal.DisplayMode = System.Windows.Controls.CalendarMode.Decade;
        }

        private void UpdateStandardLabel(object sender, RoutedEventArgs e)
        {
            string standardLabel = MetadataLoader.ComposeStandardLabel(
                MetaDataSystemName.Text, MetadataStartDate.SelectedDate, MetadataEndDate.SelectedDate
            );

            ((CreatePackageViewModel) DataContext).MetaDataNoarkSection.StandardLabel = standardLabel;
        }
    }
}
