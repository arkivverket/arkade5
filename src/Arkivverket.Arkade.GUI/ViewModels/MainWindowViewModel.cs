using System.Diagnostics;
using System.Globalization;
using System.Windows;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.Views;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.GUI.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private static bool UiLanguageIsChanged { get; set; }

        private static CloseChildWindowEvent _closeSettingsDialogEvent;
        private static Settings _settingsDialog;
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

        private string _uiAndOutputLanguagesIsDifferentWarningMessage;
        public string UiAndOutputLanguagesIsDifferentWarningMessage
        {
            get => _uiAndOutputLanguagesIsDifferentWarningMessage;
            set => SetProperty(ref _uiAndOutputLanguagesIsDifferentWarningMessage, value);
        }

        private Visibility _uiAndOutputLanguagesIsDifferentWarningMessageVisibility;
        public Visibility UiAndOutputLanguagesIsDifferentWarningMessageVisibility
        {
            get => _uiAndOutputLanguagesIsDifferentWarningMessageVisibility;
            set => SetProperty(ref _uiAndOutputLanguagesIsDifferentWarningMessageVisibility, value);
        }

        public MainWindowViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ArkadeVersion arkadeVersion)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowToolsDialogCommand = new DelegateCommand(ShowToolsDialog);
            ShowWebPageCommand = new DelegateCommand(ShowWebPage);
            ShowSettingsCommand = new DelegateCommand(ShowSettings, CanChangeSettings);
            ShowAboutDialogCommand = new DelegateCommand(ShowAboutDialog);
            ShowInvalidProcessingAreaLocationDialogCommand =
                new DelegateCommand(ShowInvalidProcessingAreaLocationDialog);
            CurrentVersion = Languages.GUI.VersionText + ArkadeVersion.Current;
            VersionStatusMessage = arkadeVersion.UpdateIsAvailable() ? Languages.GUI.NewVersionMessage : null;
            DownloadNewVersionCommand = new DelegateCommand(DownloadNewVersion);

            _closeSettingsDialogEvent = eventAggregator.GetEvent<CloseChildWindowEvent>();

            eventAggregator.GetEvent<UpdateUiLanguageEvent>()
                .Subscribe(HandleUpdatedUiLanguage, ThreadOption.UIThread);
            eventAggregator.GetEvent<UpdateOutputLanguageEvent>()
                .Subscribe(HandleUpdatedOutputLanguage, ThreadOption.UIThread);

            SetUiAndOutputLanguagesIsDifferentWarningMessageVisibility();
        }

        private void HandleUpdatedUiLanguage(string previousCultureInfoName)
        {
            UiLanguageIsChanged = previousCultureInfoName != Properties.Settings.Default.SelectedUILanguage;
            SetUiAndOutputLanguagesIsDifferentWarningMessageVisibility();
        }

        private void HandleUpdatedOutputLanguage()
        {
            SetUiAndOutputLanguagesIsDifferentWarningMessageVisibility();
        }

        private void SetUiAndOutputLanguagesIsDifferentWarningMessageVisibility()
        {
            string outputLanguageName =
                CultureInfo.CreateSpecificCulture(Properties.Settings.Default.SelectedOutputLanguage).NativeName;

            UiAndOutputLanguagesIsDifferentWarningMessage =
                string.Format(
                    Languages.GUI.UiAndOutputLanguagesIsDifferentWarningMessage,
                    outputLanguageName.Remove(outputLanguageName.IndexOf('('))
                );

            UiAndOutputLanguagesIsDifferentWarningMessageVisibility =
                Properties.Settings.Default.SelectedUILanguage == Properties.Settings.Default.SelectedOutputLanguage
                    ? Visibility.Hidden
                    : Visibility.Visible;
        }

        private static bool CanChangeSettings()
        {
            return !ArkadeProcessingState.TestingIsStarted && !ArkadeProcessingState.PackingIsStarted;
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
            _closeSettingsDialogEvent.Subscribe(CloseSettingsWindow, ThreadOption.UIThread, false);

            _settingsDialog = new Settings();
            _settingsDialog.ShowDialog();

            if (!ArkadeProcessingAreaLocationSetting.IsValid())
                ShowInvalidProcessingAreaLocationDialog();
            
            _closeSettingsDialogEvent.Unsubscribe(CloseSettingsWindow);
            RestartArkadeIfNeededAndWanted();
        }

        private static void ShowAboutDialog()
        {
            new AboutDialog().ShowDialog();
        }

        private static void CloseSettingsWindow()
        {
            _settingsDialog.Close();
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
            bool restartIsNeeded = !ArkadeProcessingAreaLocationSetting.IsApplied() || UiLanguageIsChanged;

            if (restartIsNeeded)
            {
                bool restartIsWanted = MessageBox.Show(
                                           Languages.GUI.RestartArkadeForChangesToTakeEffectPrompt,
                                           Languages.GUI.RestartArkadeDialogTitle,
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
                        MessageBox.Show(Languages.GUI.RestartFailedMessageBoxText,
                            Languages.GUI.RestartFailedMessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
            ArkadeConstants.ArkadeWebSiteUrl.LaunchUrl();
        }
    }

    public class UpdateUiLanguageEvent : PubSubEvent<string> { }
    public class UpdateOutputLanguageEvent : PubSubEvent { }
    public class CloseChildWindowEvent : PubSubEvent { }
}
