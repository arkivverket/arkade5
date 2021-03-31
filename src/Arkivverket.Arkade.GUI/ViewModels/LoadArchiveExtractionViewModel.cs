using System.IO;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class LoadArchiveExtractionViewModel : BindableBase, INavigationAware
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        private string _archiveFileName;
        private string _archiveFileNameGuiRepresentation = LoadArchiveExtractionGUI.ChooseArchiveLabelText;
        private bool _isArchiveFileNameSelected;
        private ArchiveType? _archiveType;
        private bool _isArchiveTypeSelected;
        private IArchiveTypeIdentifier _archiveTypeIdentifier;

        public string ArchiveFileName
        {
            get => _archiveFileName;
            set
            {
                SetProperty(ref _archiveFileName, value);
                NavigateToTestRunnerCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileNameGuiRepresentation
        {
            get => _archiveFileNameGuiRepresentation;
            set
            {
                SetProperty(ref _archiveFileNameGuiRepresentation, value);
                NavigateToTestRunnerCommand.RaiseCanExecuteChanged();
                IsArchiveFileNameSelected = !string.IsNullOrWhiteSpace(value);
            }
        }

        public bool IsArchiveFileNameSelected
        {
            get => _isArchiveFileNameSelected;
            set => SetProperty(ref _isArchiveFileNameSelected, value);
        }

        public ArchiveType? ArchiveType
        {
            get => _archiveType;
            set
            {
                SetProperty(ref _archiveType, value);
                _isArchiveTypeSelected = value != null;
                NavigateToTestRunnerCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand NavigateToTestRunnerCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }
        public DelegateCommand OpenArchiveFolderCommand { get; set; }

        public LoadArchiveExtractionViewModel(IRegionManager regionManager, IArchiveTypeIdentifier archiveTypeIdentifier)
        {
            _regionManager = regionManager;
            _archiveTypeIdentifier = archiveTypeIdentifier;
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            OpenArchiveFolderCommand = new DelegateCommand(OpenArchiveFolderDialog);

            NavigateToTestRunnerCommand = new DelegateCommand(NavigateToTestRunner, CanRunTests);
            _isArchiveTypeSelected = false;
        }

        private void NavigateToTestRunner()
        {
            _log.Information("User action: Navigate to test runner window with archive file {ArchiveFile} and archive type {ArchiveType}", ArchiveFileName, ArchiveType);

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("archiveFileName", ArchiveFileName);
            navigationParameters.Add("archiveType", ArchiveType);

            _regionManager.RequestNavigate("MainContentRegion", "TestRunner", navigationParameters);
        }

        private bool CanRunTests()
        {
            return !string.IsNullOrEmpty(_archiveFileName) && _isArchiveTypeSelected;
        }

        private void OpenArchiveFileDialog()
        {
            _log.Information("User action: Open archive file dialog");

            if (!OpenFileDialog(out string archiveFileName))
                return;

            ArchiveFileName = archiveFileName;

            _log.Information("User action: Choose archive file {ArchiveFileName}", ArchiveFileName);

            PresentChosenArchiveInGui(ArchiveFileName, false);

            IdentifyTypeOfChosenArchive(ArchiveFileName, false);
        }

        private void OpenArchiveFolderDialog()
        {
            _log.Information("User action: Open archive folder dialog");

            if (!OpenFolderDialog(out string archiveFolderName))
                return;

            ArchiveFileName = archiveFolderName;

            _log.Information("User action: Choose archive folder {ArchiveFileName}", ArchiveFileName);

            PresentChosenArchiveInGui(ArchiveFileName, true);

            IdentifyTypeOfChosenArchive(ArchiveFileName, true);
        }


        private bool OpenFolderDialog(out string fullFolderPath)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                fullFolderPath = dialog.SelectedPath;
                return true;
            }

            fullFolderPath = null;
            return false;
        }

        private bool OpenFileDialog(out string fullFilePath)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = LoadArchiveExtractionGUI.ChooseArchiveFileDialogFilter;
            DialogResult dialogResult = dialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                fullFilePath = dialog.FileName;
                return true;
            }

            fullFilePath = null;
            return false;
        }

        private void PresentChosenArchiveInGui(string archiveFileName, bool isDirectory)
        {
            ArchiveFileNameGuiRepresentation = isDirectory
                ? new DirectoryInfo(archiveFileName).FullName
                : Path.GetFullPath(archiveFileName);
        }

        private void IdentifyTypeOfChosenArchive(string archiveFileName, bool isDirectory)
        {
            ArchiveType = isDirectory
                ? _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(archiveFileName)
                : _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(archiveFileName);

            if (ArchiveType == null)
            {
                _log.Information("Arkade archive type detection failed.");
                _isArchiveTypeSelected = false;
            }
            else
            {
                _log.Information($"Arkade determined selected archive to be of type {ArchiveType}");
                _isArchiveTypeSelected = true;
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ArkadeProcessingState.Reset();
            MainWindowViewModel.ShowSettingsCommand.RaiseCanExecuteChanged();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}