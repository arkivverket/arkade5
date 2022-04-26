using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Util;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using MessageBox = System.Windows.MessageBox;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    class ToolsDialogViewModel : BindableBase
    {
        private readonly ILogger _log = Log.ForContext<ToolsDialogViewModel>();

        private ArkadeApi _arkadeApi;

        // ---------- File format analysis --------------

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

        private Visibility _progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetProperty(ref _progressBarVisibility, value);
        }

        public DelegateCommand ChooseDirectoryForFormatCheckCommand { get; }
        public DelegateCommand RunFormatCheckCommand { get; }


        // ---------- Archive format validation ----------

        private string _archiveFormatValidationItemPath;
        public string ArchiveFormatValidationItemPath
        {
            get => _archiveFormatValidationItemPath;
            set => SetProperty(ref _archiveFormatValidationItemPath, value);
        }

        private string[] _archiveFormatValidationFormats;
        public string[] ArchiveFormatValidationFormats
        {
            get => _archiveFormatValidationFormats;
            set => SetProperty(ref _archiveFormatValidationFormats, value);
        }

        private string _archiveFormatValidationFormat;
        public string ArchiveFormatValidationFormat
        {
            get => _archiveFormatValidationFormat;
            set => SetProperty(ref _archiveFormatValidationFormat, value);
        }

        private bool _validateArchiveFormatButtonIsEnabled;
        public bool ValidateArchiveFormatButtonIsEnabled
        {
            get => _validateArchiveFormatButtonIsEnabled;
            set => SetProperty(ref _validateArchiveFormatButtonIsEnabled, value);
        }

        private ArchiveFormatValidationStatusDisplay _archiveFormatValidationStatusDisplay;
        public ArchiveFormatValidationStatusDisplay ArchiveFormatValidationStatusDisplay
        {
            get => _archiveFormatValidationStatusDisplay;
            set => SetProperty(ref _archiveFormatValidationStatusDisplay, value);
        }

        public DelegateCommand ChooseFileForArchiveFormatValidationCommand { get; }
        public DelegateCommand ValidateArchiveFormatCommand { get; }

        // -----------------------------------------------

        private bool _closeButtonIsEnabled;
        public bool CloseButtonIsEnabled
        {
            get => _closeButtonIsEnabled;
            set => SetProperty(ref _closeButtonIsEnabled, value);
        }

        public ToolsDialogViewModel(ArkadeApi arkadeApi)
        {
            _arkadeApi = arkadeApi;

            // ---------- File format analysis --------------

            ChooseDirectoryForFormatCheckCommand = new DelegateCommand(ChooseDirectoryForFormatCheck);
            RunFormatCheckCommand = new DelegateCommand(RunFormatCheck);

            RunButtonIsEnabled = false;
            _progressBarVisibility = Visibility.Hidden;

            // ---------- Archive format validation ----------

            ChooseFileForArchiveFormatValidationCommand = new DelegateCommand(ChooseFileForArchiveFormatValidation);
            ValidateArchiveFormatCommand = new DelegateCommand(ValidateArchiveFormat);

            ValidateArchiveFormatButtonIsEnabled = false;
            _archiveFormatValidationStatusDisplay = new ArchiveFormatValidationStatusDisplay();
            ArchiveFormatValidationFormats = typeof(ArchiveFormat).GetDescriptions();

            // -----------------------------------------------

            CloseButtonIsEnabled = true;
        }

        public void OnClose(object sender, CancelEventArgs e)
        {
            const string processName = "siegfried";

            if (ExternalProcessManager.HasActiveProcess(processName))
            {
                MessageBoxResult dialogResult = MessageBox.Show(ToolsGUI.UnsavedResultsOnExitWarning,
                    "NB!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (dialogResult == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    ExternalProcessManager.Terminate(processName);
            }
        }

        // ---------- File format analysis --------------

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
                    Path.GetFileName(DirectoryForFormatCheck.TrimEnd(Path.GetInvalidFileNameChars()))
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

            var successfulRun = true;

            try
            {
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
            }
            catch (Exception e)
            {
                _log.Information("Format analysis failed: " + e.Message);
                successfulRun = false;
            }

            CloseButtonIsEnabled = true;
            ProgressBarVisibility = Visibility.Hidden;

            if (!successfulRun)
                return;

            FormatCheckStatus = $"{ToolsGUI.FormatCheckCompletedMessage}\n" +
                                $"{filePath}";

            string argument = "/select, \"" + filePath + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        // ---------- Archive format validation ----------

        private void ChooseFileForArchiveFormatValidation()
        {
            FilePicker("archive format validation",
                ToolsGUI.ArchiveFormatValidationFileSelectDialogTitle,
                ToolsGUI.ArchiveFormatValidationFileSelectDialogFilter,
                out string fileToValidate
            );

            if (fileToValidate != null)
            {
                ArchiveFormatValidationItemPath = fileToValidate;
                ValidateArchiveFormatButtonIsEnabled = true;
            }
        }

        private async void ValidateArchiveFormat()
        {
            CloseButtonIsEnabled = false;
            ValidateArchiveFormatButtonIsEnabled = false;
            ArchiveFormatValidationStatusDisplay.DisplayRunning();

            FileSystemInfo item = new FileInfo(ArchiveFormatValidationItemPath);
            ArchiveFormat format = ArchiveFormatValidationFormat.GetValueByDescription<ArchiveFormat>();
            SupportedLanguage language = LanguageSettingHelper.GetUILanguage();

            var isBatchValidation = false; // TODO: So ...

            ArchiveFormatValidationResponse response = await _arkadeApi.ValidateArchiveFormat(item, format, language, isBatchValidation);

            ArchiveFormatValidationStatusDisplay.DisplayFinished(response);
            ValidateArchiveFormatButtonIsEnabled = true;
            CloseButtonIsEnabled = true;
        }
        
        // -----------------------------------------------

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

        private void FilePicker(string action, string title, string filter, out string file)
        {
            _log.Information($"User action: Open choose file for {action} dialog");

            var selectFileDialog = new OpenFileDialog()
            {
                Title = title,
                Filter = filter,
            };


            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = selectFileDialog.FileName;

                _log.Information($"User action: Chose file for {action}: {file}");
            }
            else
            {
                file = null;
                _log.Information($"User action: Abort choose file for {action}");
            }
        }
    }
}
