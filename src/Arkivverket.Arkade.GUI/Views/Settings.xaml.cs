using System.Windows;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
         }
    }
}
