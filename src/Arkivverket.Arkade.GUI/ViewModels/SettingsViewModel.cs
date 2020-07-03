using System;
using Arkivverket.Arkade.Core.Base;
﻿using System.Windows;
using Arkivverket.Arkade.GUI.Properties;
using Arkivverket.Arkade.GUI.Util;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ILogger _log = Log.ForContext<SettingsViewModel>();
        private string _arkadeProcessingAreaLocationSetting;
        private bool _darkModeSelected;
        public string CurrentArkadeProcessingAreaLocation { get; }
        public string DirectoryNameArkadeProcessingAreaRoot { get; }

        public bool DarkModeSelected
        {
            get => _darkModeSelected;
            set => SetProperty(ref _darkModeSelected, value);
        }

        public string ArkadeProcessingAreaLocationSetting
        {
            get { return _arkadeProcessingAreaLocationSetting; }
            set { SetProperty(ref _arkadeProcessingAreaLocationSetting, value); }
        }

        public DelegateCommand ChangeArkadeProcessingAreaLocationCommand { get; }
        public DelegateCommand ApplyChangesCommand { get; }

        public SettingsViewModel()
        {
            ArkadeProcessingAreaLocationSetting = Util.ArkadeProcessingAreaLocationSetting.Get();
            CurrentArkadeProcessingAreaLocation = ArkadeProcessingArea.Location?.FullName;
            DirectoryNameArkadeProcessingAreaRoot = Arkade.Core.Util.ArkadeConstants.DirectoryNameArkadeProcessingAreaRoot;

            ChangeArkadeProcessingAreaLocationCommand = new DelegateCommand(ChangeArkadeProcessingAreaLocation);
            ApplyChangesCommand = new DelegateCommand(ApplyChanges);

            DarkModeSelected = Settings.Default.DarkModeEnabled;
        }

        private void ChangeArkadeProcessingAreaLocation()
        {
            if (!ArkadeInstance.IsOnlyInstance)
            {
                string message = Resources.SettingsGUI.OtherInstancesRunningOnProcessingAreaChangeMessage;
                MessageBox.Show(message, "NB!", MessageBoxButton.OK, MessageBoxImage.Error);

                _log.Information("Arkade processing area location change denied due to other running Arkade instances");

                return;
            }

            _log.Information("User action: Open choose Arkade processing area location dialog");

            var selectDirectoryDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = ArkadeProcessingAreaLocationSetting
            };

            if (selectDirectoryDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ArkadeProcessingAreaLocationSetting = selectDirectoryDialog.FileName;

                _log.Information(
                    "User action: Choose Arkade processing area location {ArkadeDirectoryLocationSetting}",
                    ArkadeProcessingAreaLocationSetting
                );
            }
            else _log.Information("User action: Abort choose Arkade processing area location");
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

        private void ApplyChanges()
        {
            ApplySelectedMode();
            Util.ArkadeProcessingAreaLocationSetting.Set(ArkadeProcessingAreaLocationSetting);
        }
    }
}
