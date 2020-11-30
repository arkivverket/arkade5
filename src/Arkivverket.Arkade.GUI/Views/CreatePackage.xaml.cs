using Arkivverket.Arkade.GUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            var datepicker = (DatePicker)sender;
            var popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
            var cal = (Calendar)popup.Child;
            cal.DisplayMode = CalendarMode.Decade;
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
