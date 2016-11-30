using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using System;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class LoadArchiveExtractionViewModel : BindableBase, INavigationAware
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        private string _archiveFileName;
        private string _metadataFileName;
        private ArchiveType _archiveType;
        private bool _isArchiveTypeSelected;

        // TODO: MetadataFileName is replaced by GUI selection of archiveType ... leaving code in case needed for METS based input meta file
        public string MetadataFileName
        {
            get { return _metadataFileName; }
            set
            {
                SetProperty(ref _metadataFileName, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileName
        {
            get { return _archiveFileName; }
            set
            {
                SetProperty(ref _archiveFileName, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }

        public ArchiveType ArchiveType
        {
            get { return _archiveType; }
            set
            {
                SetProperty(ref _archiveType, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }


        public DelegateCommand NavigateCommand { get; set; }
        public DelegateCommand OpenMetadataFileCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }
        public DelegateCommand OpenArchiveFolderCommand { get; set; }
        public DelegateCommand<string> SetArchiveTypeCommand { get; set; } // Would be better to user ArchiveType enum as arg, but could not get to work with Prism

        public LoadArchiveExtractionViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OpenMetadataFileCommand = new DelegateCommand(OpenMetadataFileDialog);
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            OpenArchiveFolderCommand = new DelegateCommand(OpenArchiveFolderDialog);
            SetArchiveTypeCommand = new DelegateCommand<string>(SetArchiveTypeUserInput); 

            NavigateCommand = new DelegateCommand(Navigate, CanRunTests);
            _isArchiveTypeSelected = false;
        }

        private void SetArchiveTypeUserInput(string archiveTypeSelected)
        {
            _log.Debug($"User set archive type to {archiveTypeSelected}");

            ArchiveType tempArchiveType;
            if (ArchiveType.TryParse(archiveTypeSelected, true, out tempArchiveType))
            {
                _isArchiveTypeSelected = true;
                ArchiveType = tempArchiveType;
            }
        }

        private void Navigate()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("archiveFileName", ArchiveFileName);
            navigationParameters.Add("archiveType", ArchiveType);

            _log.Debug("Navigating to TestRunner window with archive file {ArchiveFile} and archive type {ArchiveType}", ArchiveFileName, ArchiveType);

            _regionManager.RequestNavigate("MainContentRegion", "TestRunner", navigationParameters);
        }

        private bool CanRunTests()
        {
            return !string.IsNullOrEmpty(_archiveFileName) && _isArchiveTypeSelected;
        }

        private void OpenMetadataFileDialog()
        {
            MetadataFileName = OpenFileDialog();
        }


        private void OpenArchiveFileDialog()
        {
            ArchiveFileName = OpenFileDialog();

            if (ArchiveFileName == null)
            {
                MetadataFileName = null;
                return;
            }

            string infoXmlFileName = Path.Combine(new FileInfo(ArchiveFileName).Directory?.FullName, ArkadeConstants.InfoXmlFileName);
            if (File.Exists(infoXmlFileName))
            {
                MetadataFileName = infoXmlFileName;
            }
            else
            {
                MetadataFileName = null;
            }
        }

        private void OpenArchiveFolderDialog()
        {
            ArchiveFileName = OpenFolderDialog();

            if (ArchiveFileName == null)
            {
                MetadataFileName = null;
                return;
            }

            string infoXmlFileName = Path.Combine(new DirectoryInfo(ArchiveFileName).Parent?.FullName, ArkadeConstants.InfoXmlFileName);
            if (File.Exists(infoXmlFileName))
            {
                MetadataFileName = infoXmlFileName;
            }
            else
            {
                MetadataFileName = null;
            }
        }


        private string OpenFolderDialog()
        {
            string selected = null;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
                //Title = ""
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selected = dialog.FileName;
            }

            return selected;
        }

        private string OpenFileDialog()
        {
            string selectedFileName = null;
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result == true)
            {
                selectedFileName = dialog.FileName;
            }
            return selectedFileName;
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