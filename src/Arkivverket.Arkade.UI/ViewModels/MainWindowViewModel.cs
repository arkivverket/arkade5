using System.Windows.Forms;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Resources;
using Arkivverket.Arkade.UI.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateCommandMain { get; set; }
        public DelegateCommand ShowUserGuideCommand { get; set; }
        public static DelegateCommand ShowSettingsCommand { get; set; }
        public DelegateCommand HandleUndefinedProcessingAreaLocationCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowUserGuideCommand = new DelegateCommand(ShowUserGuide);
            ShowSettingsCommand = new DelegateCommand(ShowSettings);
            HandleUndefinedProcessingAreaLocationCommand = new DelegateCommand(HandleUndefinedProcessingAreaLocation);
        }

        private static void HandleUndefinedProcessingAreaLocation()
        {
            if (!ArkadeProcessingArea.HasValidLocation())
            {
                MessageBox.Show(
                    SettingsUI.UndefinedArkadeProcessingAreaLocationDialogMessage,
                    SettingsUI.UndefinedArkadeProcessingAreaLocationDialogTitle,
                    MessageBoxButtons.OK
                );

                ShowSettingsCommand.Execute();
            }
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("MainContentRegion", uri);
        }

        private void ShowUserGuide()
        {
            System.Diagnostics.Process.Start("http://arkade.arkivverket.no/no/latest/Brukerveiledning.html");
        }

        private static void ShowSettings()
        {
            new Settings().ShowDialog();

            RestartArkadeIfNeededAndWanted();

            HandleUndefinedProcessingAreaLocation();
        }

        private static void RestartArkadeIfNeededAndWanted()
        {
            bool restartIsNeeded = ArkadeStatus.RestartIsNeeded;

            if (restartIsNeeded)
            {
                bool restartIsWanted = MessageBox.Show(
                                           Resources.UI.RestartArkadeForChangesToTakeEffectPrompt,
                                           Resources.UI.RestartArkadeDialogTitle,
                                           MessageBoxButtons.YesNo) == DialogResult.Yes;

                if (restartIsWanted)
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
    }
}
