using System.Windows.Forms;
using Arkivverket.Arkade.UI.Resources;
using Arkivverket.Arkade.UI.Util;
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
        public DelegateCommand ShowInvalidProcessingAreaLocationDialogCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowUserGuideCommand = new DelegateCommand(ShowUserGuide);
            ShowSettingsCommand = new DelegateCommand(ShowSettings);
            ShowInvalidProcessingAreaLocationDialogCommand =
                new DelegateCommand(ShowInvalidProcessingAreaLocationDialog);
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

            if (!ArkadeProcessingAreaLocationSetting.IsValid())
                ShowInvalidProcessingAreaLocationDialog();
            
            RestartArkadeIfNeededAndWanted();
        }

        private static void ShowInvalidProcessingAreaLocationDialog()
        {
            DialogResult dialogResult = MessageBox.Show(
                SettingsUI.UndefinedArkadeProcessingAreaLocationDialogMessage,
                SettingsUI.UndefinedArkadeProcessingAreaLocationDialogTitle,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation
            );

            if (dialogResult == DialogResult.OK)
                ShowSettingsCommand.Execute();
            else
                System.Windows.Application.Current.Shutdown();
        }

        private static void RestartArkadeIfNeededAndWanted()
        {
            bool restartIsNeeded = !ArkadeProcessingAreaLocationSetting.IsApplied();

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
