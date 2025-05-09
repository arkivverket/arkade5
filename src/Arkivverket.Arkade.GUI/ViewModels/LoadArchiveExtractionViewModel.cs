using System;
using System.IO;
using System.Windows.Forms;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
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
        private readonly ArkadeCoreApi _arkadeCoreApi;
        private FileSystemInfo _archiveSource;
        private ArchiveProcessing _archiveProcessing;

        public string ArchiveFileName
        {
            get => _archiveFileName;
            set
            {
                SetProperty(ref _archiveFileName, value);
                LoadSelectedArchiveInputCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileNameGuiRepresentation
        {
            get => _archiveFileNameGuiRepresentation;
            set
            {
                SetProperty(ref _archiveFileNameGuiRepresentation, value);
                LoadSelectedArchiveInputCommand.RaiseCanExecuteChanged();
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
                LoadSelectedArchiveInputCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand LoadSelectedArchiveInputCommand { get; set; }
        public DelegateCommand NavigateToTestRunnerCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }
        public DelegateCommand OpenArchiveFolderCommand { get; set; }

        public LoadArchiveExtractionViewModel(IRegionManager regionManager, IArchiveTypeIdentifier archiveTypeIdentifier, ArkadeCoreApi arkadeCoreApi)
        {
            _arkadeCoreApi = arkadeCoreApi;
            _regionManager = regionManager;
            _archiveTypeIdentifier = archiveTypeIdentifier;
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            OpenArchiveFolderCommand = new DelegateCommand(OpenArchiveFolderDialog);

            LoadSelectedArchiveInputCommand = new DelegateCommand(LoadSelectedArchiveInput, CanLoadSelectedArchiveInput);
            NavigateToTestRunnerCommand = new DelegateCommand(NavigateToTestRunner, CanRunTests);
            _isArchiveTypeSelected = false;
        }

        private void LoadSelectedArchiveInput()
        {
            _archiveProcessing = new ArchiveProcessing();

            var archiveType = (ArchiveType)ArchiveType;

            if (_archiveSource is FileInfo { Extension: ".tar" } tarFile)
                _archiveProcessing.InputDiasPackage =
                    _arkadeCoreApi.LoadDiasPackage(tarFile, archiveType, _archiveProcessing.ProcessingDirectory);
            else
                _archiveProcessing.Archive = _arkadeCoreApi.LoadArchiveExtraction(_archiveSource, archiveType);

            if(NavigateToTestRunnerCommand.CanExecute())
                NavigateToTestRunnerCommand.Execute();
        }

        private bool CanLoadSelectedArchiveInput()
        {
            return !string.IsNullOrEmpty(_archiveFileName) && _isArchiveTypeSelected;
        }

        private void NavigateToTestRunner()
        {
            _log.Information("User action: Navigate to test runner window with archive file {ArchiveFile} and archive type {ArchiveType}", ArchiveFileName, ArchiveType);

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("archiveProcessing", _archiveProcessing);

            _regionManager.RequestNavigate("MainContentRegion", "TestRunner", navigationParameters);
        }

        private bool CanRunTests()
        {
            return TestSession.IsTestableArchive(_archiveProcessing.Archive, null, out _);
        }

        private void OpenArchiveFileDialog()
        {
            _log.Information("User action: Open archive file dialog");

            if (!OpenFileDialog(out string archiveFileName))
                return;

            ArchiveFileName = archiveFileName;
            _archiveSource = new FileInfo(archiveFileName);

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
            _archiveSource = new DirectoryInfo(archiveFolderName);

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