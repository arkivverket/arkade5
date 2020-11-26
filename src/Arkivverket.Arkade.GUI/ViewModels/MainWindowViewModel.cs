using System.Diagnostics;
using System.Windows;
using Arkivverket.Arkade.GUI.Resources;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.Views;
using Arkivverket.Arkade.Core.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateCommandMain { get; set; }
        public DelegateCommand ShowToolsDialogCommand { get; set; }
        public DelegateCommand ShowWebPageCommand { get; set; }
        public static DelegateCommand ShowSettingsCommand { get; set; }
        public DelegateCommand ShowAboutDialogCommand { get; set; }
        public DelegateCommand ShowInvalidProcessingAreaLocationDialogCommand { get; }
        public string CurrentVersion { get; }
        public string VersionStatusMessage { get; }
        public DelegateCommand DownloadNewVersionCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager, ArkadeVersion arkadeVersion)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowToolsDialogCommand = new DelegateCommand(ShowToolsDialog);
            ShowWebPageCommand = new DelegateCommand(ShowWebPage);
            ShowSettingsCommand = new DelegateCommand(ShowSettings);
            ShowAboutDialogCommand = new DelegateCommand(ShowAboutDialog);
            ShowInvalidProcessingAreaLocationDialogCommand =
                new DelegateCommand(ShowInvalidProcessingAreaLocationDialog);
            CurrentVersion = "Versjon " + ArkadeVersion.Current;
            VersionStatusMessage = arkadeVersion.UpdateIsAvailable() ? Resources.GUI.NewVersionMessage : null;
            DownloadNewVersionCommand = new DelegateCommand(DownloadNewVersion);
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("MainContentRegion", uri);
        }

        private static void ShowToolsDialog()
        {
            new ToolsDialog().ShowDialog();
        }

        private static void ShowWebPage()
        {
            LaunchArkadeWebSite();
        }

        private static void ShowSettings()
        {
            new Settings().ShowDialog();

            if (!ArkadeProcessingAreaLocationSetting.IsValid())
                ShowInvalidProcessingAreaLocationDialog();
            
            RestartArkadeIfNeededAndWanted();
        }

        private static void ShowAboutDialog()
        {
            new AboutDialog().ShowDialog();
        }

        private static void ShowInvalidProcessingAreaLocationDialog()
        {
            MessageBoxResult dialogResult = MessageBox.Show(
                SettingsGUI.UndefinedArkadeProcessingAreaLocationDialogMessage,
                SettingsGUI.UndefinedArkadeProcessingAreaLocationDialogTitle,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Exclamation
            );

            if (dialogResult == MessageBoxResult.OK)
                ShowSettingsCommand.Execute();
            else
                Application.Current.Shutdown();
        }

        private static void RestartArkadeIfNeededAndWanted()
        {
            bool restartIsNeeded = !ArkadeProcessingAreaLocationSetting.IsApplied();

            if (restartIsNeeded)
            {
                bool restartIsWanted = MessageBox.Show(
                                           Resources.GUI.RestartArkadeForChangesToTakeEffectPrompt,
                                           Resources.GUI.RestartArkadeDialogTitle,
                                           MessageBoxButton.YesNo) == MessageBoxResult.Yes;

                if (restartIsWanted)
                {
                    string mainModuleFileName = Process.GetCurrentProcess().MainModule?.FileName;

                    if (mainModuleFileName != null)
                    {
                        Process.Start(mainModuleFileName);
                    }
                    else
                    {
                        MessageBox.Show("Could not restart Arkade\nPlease manually start Arkade after shutdown",
                            "Automatic restart failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    Application.Current.Shutdown();
                }
            }
        }

        private static void DownloadNewVersion()
        {
            LaunchArkadeWebSite();
        }

        private static void LaunchArkadeWebSite()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://arkade.arkivverket.no/",
                UseShellExecute = true
            });
        }
    }
}
