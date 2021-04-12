using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Util;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    class ToolsDialogViewModel : BindableBase
    {
        private ArkadeApi _arkadeApi;

        private string _directoryForFormatCheck;
        public string DirectoryForFormatCheck
        {
            get => _directoryForFormatCheck;
            set => SetProperty(ref _directoryForFormatCheck, value);
        }

        private string _directoryToSaveFormatCheckResult;
        public string DirectoryToSaveFormatCheckResult
        {
            get => _directoryToSaveFormatCheckResult;
            set => SetProperty(ref _directoryToSaveFormatCheckResult, value);
        }

        private string _formatCheckStatus;
        public string FormatCheckStatus
        {
            get => _formatCheckStatus;
            set => SetProperty(ref _formatCheckStatus, value);
        }

        private bool _runButtonIsEnabled;
        public bool RunButtonIsEnabled
        {
            get => _runButtonIsEnabled;
            set => SetProperty(ref _runButtonIsEnabled, value);
        }

        private bool _closeButtonIsEnabled;
        public bool CloseButtonIsEnabled
        {
            get => _closeButtonIsEnabled;
            set => SetProperty(ref _closeButtonIsEnabled, value);
        }

        private Visibility _progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetProperty(ref _progressBarVisibility, value);
        }

        private readonly ILogger _log = Log.ForContext<ToolsDialogViewModel>();
        public DelegateCommand ChooseDirectoryForFormatCheckCommand { get; }
        public DelegateCommand RunFormatCheckCommand { get; }

        public ToolsDialogViewModel(ArkadeApi arkadeApi)
        {
            _arkadeApi = arkadeApi;

            ChooseDirectoryForFormatCheckCommand = new DelegateCommand(ChooseDirectoryForFormatCheck);
            RunFormatCheckCommand = new DelegateCommand(RunFormatCheck);

            RunButtonIsEnabled = false;
            CloseButtonIsEnabled = true;
            _progressBarVisibility = Visibility.Hidden;
        }

        private void ChooseDirectoryForFormatCheck()
        {
            FormatCheckStatus = string.Empty;

            DirectoryPicker("format analysis",
                ToolsGUI.ChooseDirectoryToAnalyse,
                out string directoryForFormatCheck
            );

            if (directoryForFormatCheck != null)
            {
                DirectoryForFormatCheck = directoryForFormatCheck;
                RunButtonIsEnabled = true;
            }
        }

        private async void RunFormatCheck()
        {
            const string action = "save format analysis result";

            _log.Information($"User action: Open choose directory for {action} dialog");

            var saveFileDialog = new SaveFileDialog
            {
                Title = ToolsGUI.FormatCheckOutputDirectoryPickerTitle,
                DefaultExt = "csv",
                AddExtension = true,
                Filter = ToolsGUI.SaveFormatFileExtensionFilter,
                FileName = string.Format(
                    OutputFileNames.FileFormatInfoFile,
                    Path.GetFileName(DirectoryForFormatCheck)
                )
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                _log.Information($"User action: Abort choose directory for {action}");
                return;
            }

            string filePath = saveFileDialog.FileName;

            _log.Information($"User action: Chose directory for {action}: {filePath}");

            DirectoryToSaveFormatCheckResult = Path.GetDirectoryName(filePath);

            await Task.Run(
                () =>
                {
                    RunButtonIsEnabled = false;
                    CloseButtonIsEnabled = false;
                    ProgressBarVisibility = Visibility.Visible;

                    SupportedLanguage language = LanguageSettingHelper.GetOutputLanguage();

                    _arkadeApi.GenerateFileFormatInfoFiles(new DirectoryInfo(DirectoryForFormatCheck),
                        DirectoryToSaveFormatCheckResult, Path.GetFileName(filePath), language);
                });

            CloseButtonIsEnabled = true;
            ProgressBarVisibility = Visibility.Hidden;

            FormatCheckStatus = $"{ToolsGUI.FormatCheckCompletedMessage}\n" +
                                $"{filePath}";

            string argument = "/select, \"" + filePath + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void DirectoryPicker(string action, string title, out string directory)
        {
            _log.Information($"User action: Open choose directory for {action} dialog");

            var selectDirectoryDialog = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
            };


            if (selectDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                directory = selectDirectoryDialog.SelectedPath;

                _log.Information($"User action: Chose directory for {action}: {directory}");
            }
            else
            {
                directory = null;
                _log.Information($"User action: Abort choose directory for {action}");
            }
        }
    }
}
