using System;
using System.Collections.Generic;
using System.Threading;
using Arkivverket.Arkade.Core.Base;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.GUI.Properties;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.Languages;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using MessageBox = System.Windows.MessageBox;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private readonly ILogger _log = Log.ForContext<SettingsViewModel>();
        private string _arkadeProcessingAreaLocationSetting;
        private bool _darkModeSelected;
        private SupportedLanguage _selectedUILanguage;
        private SupportedLanguage _selectedOutputLanguage;

        public string CurrentArkadeProcessingAreaLocation { get; }
        public string DirectoryNameArkadeProcessingAreaRoot { get; }
        public Dictionary<SupportedLanguage, string> LanguageOptions { get; }

        public bool DarkModeSelected
        {
            get => _darkModeSelected;
            set => SetProperty(ref _darkModeSelected, value);
        }

        public int SelectedUILanguageIndex
        {
            get => (int) SelectedUILanguage;
            set => SelectedUILanguage = (SupportedLanguage) value;
        }

        private SupportedLanguage SelectedUILanguage
        {
            get => _selectedUILanguage;
            set => SetProperty(ref _selectedUILanguage, value);
        }

        public int SelectedOutputLanguageIndex
        {
            get => (int)SelectedOutputLanguage;
            set => SelectedOutputLanguage = (SupportedLanguage)value;
        }
        private SupportedLanguage SelectedOutputLanguage
        {
            get => _selectedOutputLanguage;
            set => SetProperty(ref _selectedOutputLanguage, value);
        }

        public string ArkadeProcessingAreaLocationSetting
        {
            get => _arkadeProcessingAreaLocationSetting;
            set => SetProperty(ref _arkadeProcessingAreaLocationSetting, value);
        }

        public DelegateCommand ChangeArkadeProcessingAreaLocationCommand { get; }
        public DelegateCommand ApplyChangesAndCloseWindowCommand { get; }

        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            ArkadeProcessingAreaLocationSetting = Util.ArkadeProcessingAreaLocationSetting.Get();
            CurrentArkadeProcessingAreaLocation = ArkadeProcessingArea.Location?.FullName;
            DirectoryNameArkadeProcessingAreaRoot = Core.Util.ArkadeConstants.DirectoryNameArkadeProcessingAreaRoot;

            ChangeArkadeProcessingAreaLocationCommand = new DelegateCommand(ChangeArkadeProcessingAreaLocation);
            ApplyChangesAndCloseWindowCommand = new DelegateCommand(ApplyChangesAndCloseWindow);

            LanguageOptions = GetSupportedLanguagesAsString();
            SelectedOutputLanguage = LanguageSettingHelper.GetOutputLanguage();
            SelectedUILanguage = LanguageSettingHelper.GetUILanguage();

            DarkModeSelected = Settings.Default.DarkModeEnabled;
        }

        private void ChangeArkadeProcessingAreaLocation()
        {
            if (!ArkadeInstance.IsOnlyInstance)
            {
                string message = SettingsGUI.OtherInstancesRunningOnProcessingAreaChangeMessage;
                MessageBox.Show(message, "NB!", MessageBoxButton.OK, MessageBoxImage.Error);

                _log.Information("Arkade processing area location change denied due to other running Arkade instances");

                return;
            }

            _log.Information("User action: Open choose Arkade processing area location dialog");

            var selectDirectoryDialog = new FolderBrowserDialog();

            if (selectDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                ArkadeProcessingAreaLocationSetting = selectDirectoryDialog.SelectedPath;

                _log.Information(
                    "User action: Choose Arkade processing area location {ArkadeDirectoryLocationSetting}",
                    ArkadeProcessingAreaLocationSetting
                );
            }
            else _log.Information("User action: Abort choose Arkade processing area location");
        }

        private void ApplyChangesAndCloseWindow()
        {
            ApplySelectedMode();
            ApplyUILanguageSelection();
            ApplyOutputLanguageSelection();
            Util.ArkadeProcessingAreaLocationSetting.Set(ArkadeProcessingAreaLocationSetting);

            _eventAggregator.GetEvent<CloseChildWindowEvent>().Publish();
        }

        private void ApplySelectedMode()
        {
            Settings.Default.DarkModeEnabled = DarkModeSelected;

            if (DarkModeSelected)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightMode();
            }
        }

        public static void ApplyDarkMode()
        {
            App.Current.Resources.MergedDictionaries[0].Source = new Uri(
                @"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesigntheme.dark.xaml");
        }
        
        public static void ApplyLightMode()
        {
            App.Current.Resources.MergedDictionaries[0].Source = new Uri(
                @"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesigntheme.light.xaml");
        }

        private void ApplyUILanguageSelection()
        {
            string currentCulture = Settings.Default.SelectedUILanguage;

            Settings.Default.SelectedUILanguage = SelectedUILanguage.ToString();

            _eventAggregator.GetEvent<UpdateUiLanguageEvent>().Publish(currentCulture);

            Settings.Default.Save();
        }

        private void ApplyOutputLanguageSelection()
        {
            Settings.Default.SelectedOutputLanguage = SelectedOutputLanguage.ToString();

            _eventAggregator.GetEvent<UpdateOutputLanguageEvent>().Publish();

            Settings.Default.Save();
        }

        private static Dictionary<SupportedLanguage, string> GetSupportedLanguagesAsString()
        {
            var supportedLanguages = new Dictionary<SupportedLanguage, string>();
            foreach (SupportedLanguage supportedLanguage in Enum.GetValues(typeof(SupportedLanguage)))
            {
                string languageAsString = supportedLanguage switch
                {
                    SupportedLanguage.en => "English",
                    SupportedLanguage.nb => "Norsk (Bokmål)",
                    _ => null
                };
                supportedLanguages.TryAdd(supportedLanguage, languageAsString);
            }
            return supportedLanguages;
        }
    }
}
