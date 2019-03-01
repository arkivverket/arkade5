using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Windows.Navigation;
using Arkivverket.Arkade.GUI.Resources;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.Views;
using Arkivverket.Arkade.Core.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateCommandMain { get; set; }
        public DelegateCommand ShowUserGuideCommand { get; set; }
        public static DelegateCommand ShowSettingsCommand { get; set; }
        public DelegateCommand ShowInvalidProcessingAreaLocationDialogCommand { get; }
        public string CurrentVersion { get; }
        public string VersionStatusMessage { get; }
        public DelegateCommand DownloadNewVersionCommand { get; }
        public DelegateCommand VersionHistoryCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager, ArkadeVersion arkadeVersion)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowUserGuideCommand = new DelegateCommand(ShowUserGuide);
            ShowSettingsCommand = new DelegateCommand(ShowSettings);
            ShowInvalidProcessingAreaLocationDialogCommand =
                new DelegateCommand(ShowInvalidProcessingAreaLocationDialog);
            CurrentVersion = "Versjon " + ArkadeVersion.Current;
            VersionStatusMessage = arkadeVersion.UpdateIsAvailable() ? Resources.GUI.NewVersionMessage : null;
            DownloadNewVersionCommand = new DelegateCommand(DownloadNewVersion);
            VersionHistoryCommand = new DelegateCommand(VersionHistory);
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
                SettingsGUI.UndefinedArkadeProcessingAreaLocationDialogMessage,
                SettingsGUI.UndefinedArkadeProcessingAreaLocationDialogTitle,
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
                                           Resources.GUI.RestartArkadeForChangesToTakeEffectPrompt,
                                           Resources.GUI.RestartArkadeDialogTitle,
                                           MessageBoxButtons.YesNo) == DialogResult.Yes;

                if (restartIsWanted)
                {
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private static void DownloadNewVersion()
        {
            Process.Start("https://github.com/arkivverket/arkade5/releases/latest");
        }

        private static void VersionHistory()
        {
            Process.Start("https://github.com/arkivverket/arkade5/releases");
        }
    }
}
