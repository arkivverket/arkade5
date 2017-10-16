using Arkivverket.Arkade.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ILogger _log = Log.ForContext<SettingsViewModel>();
        private string _arkadeProcessingAreaLocationSetting;
        public string CurrentArkadeProcessingAreaLocation { get; }

        public string ArkadeProcessingAreaLocationSetting
        {
            get { return _arkadeProcessingAreaLocationSetting; }
            set { SetProperty(ref _arkadeProcessingAreaLocationSetting, value); }
        }

        public DelegateCommand ChangeArkadeProcessingAreaLocationCommand { get; }
        public DelegateCommand ApplyChangesCommand { get; }

        public SettingsViewModel()
        {
            ArkadeProcessingAreaLocationSetting = ArkadeProcessingArea.GetLocationSetting(); // Why is the setting needed?
            CurrentArkadeProcessingAreaLocation = ArkadeProcessingArea.Location?.FullName;

            ChangeArkadeProcessingAreaLocationCommand = new DelegateCommand(ChangeArkadeProcessingAreaLocation);
            ApplyChangesCommand = new DelegateCommand(ApplyChanges);
        }

        private void ChangeArkadeProcessingAreaLocation()
        {
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

        private void ApplyChanges()
        {
            ArkadeProcessingArea.SetLocationSetting(ArkadeProcessingAreaLocationSetting);
        }
    }
}
