using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        private readonly SettingsViewModel _settingsViewModel;

        public Settings()
        {
            InitializeComponent();

            _settingsViewModel = new SettingsViewModel();

            DataContext = _settingsViewModel;

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
         }

        private void ApplyChangesAndClose(object sender, RoutedEventArgs e)
        {
            _settingsViewModel.ApplyChangesCommand.Execute();

            Close();
        }
    }
}
