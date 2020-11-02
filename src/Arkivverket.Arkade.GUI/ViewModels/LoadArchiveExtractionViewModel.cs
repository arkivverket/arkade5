using System.IO;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
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
        private string _archiveFileNameGuiRepresentation;
        private ArchiveType? _archiveType;
        private bool _isArchiveTypeSelected;
        private IArchiveTypeIdentifier _archiveTypeIdentifier;

        public string ArchiveFileName
        {
            get => _archiveFileName;
            set
            {
                SetProperty(ref _archiveFileName, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileNameGuiRepresentation
        {
            get => _archiveFileNameGuiRepresentation;
            set
            {
                SetProperty(ref _archiveFileNameGuiRepresentation, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }


        public ArchiveType? ArchiveType
        {
            get => _archiveType;
            set
            {
                SetProperty(ref _archiveType, value);
                _isArchiveTypeSelected = value != null;
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand NavigateCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }
        public DelegateCommand OpenArchiveFolderCommand { get; set; }

        public LoadArchiveExtractionViewModel(IRegionManager regionManager, IArchiveTypeIdentifier archiveTypeIdentifier)
        {
            _regionManager = regionManager;
            _archiveTypeIdentifier = archiveTypeIdentifier;
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            OpenArchiveFolderCommand = new DelegateCommand(OpenArchiveFolderDialog);

            NavigateCommand = new DelegateCommand(Navigate, CanRunTests);
            _isArchiveTypeSelected = false;
        }

        private void Navigate()
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

            ArchiveFileName = OpenFileDialog();

            if (ArchiveFileName == null)
                return;

            _log.Information("User action: Choose archive file {ArchiveFileName}", ArchiveFileName);

            PresentChosenArchiveInGui(ArchiveFileName, false);

            IdentifyTypeOfChosenArchive(ArchiveFileName, false);
        }

        private void OpenArchiveFolderDialog()
        {
            _log.Information("User action: Open archive folder dialog");

            ArchiveFileName = OpenFolderDialog();

            if (ArchiveFileName == null)
                return;

            _log.Information("User action: Choose archive folder {ArchiveFileName}", ArchiveFileName);

            PresentChosenArchiveInGui(ArchiveFileName, true);

            IdentifyTypeOfChosenArchive(ArchiveFileName, true);
        }


        private string OpenFolderDialog()
        {
            var dialog = new FolderBrowserDialog();

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        }

        private string OpenFileDialog()
        {
            var dialog = new OpenFileDialog();

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        private void PresentChosenArchiveInGui(string archiveFileName, bool isDirectory)
        {
            if (isDirectory)
            {
                ArchiveFileNameGuiRepresentation =
                    $"{Resources.GUI.LoadArchiveSelectedFolderText}: {new DirectoryInfo(archiveFileName).FullName}";
            }
            else
            {
                ArchiveFileNameGuiRepresentation =
                    $"{Resources.GUI.LoadArchiveSelectedFileText}: {Path.GetFullPath(archiveFileName)}";
            }
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