using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using System;
using System.Windows;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class LoadArchiveExtractionViewModel : BindableBase, INavigationAware
    {
        private ILogger _log = Log.ForContext<LoadArchiveExtractionViewModel>();

        private readonly IRegionManager _regionManager;
        private string _archiveFileName;
        private string _archiveFileNameGuiRepresentation;
        private ArchiveType _archiveType;
        private bool _isArchiveTypeSelected;

        public string ArchiveFileName
        {
            get { return _archiveFileName; }
            set
            {
                SetProperty(ref _archiveFileName, value);
                NavigateCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileNameGuiRepresentation
        {
            get { return _archiveFileNameGuiRepresentation; }
            set
            {
                SetProperty(ref _archiveFileNameGuiRepresentation, value);
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
        public DelegateCommand OpenArchiveFileCommand { get; set; }
        public DelegateCommand OpenArchiveFolderCommand { get; set; }
        public DelegateCommand<string> SetArchiveTypeCommand { get; set; } // Would be better to user ArchiveType enum as arg, but could not get to work with Prism

        public LoadArchiveExtractionViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            OpenArchiveFolderCommand = new DelegateCommand(OpenArchiveFolderDialog);
            SetArchiveTypeCommand = new DelegateCommand<string>(SetArchiveTypeUserInput); 

            NavigateCommand = new DelegateCommand(Navigate, CanRunTests);
            _isArchiveTypeSelected = false;
        }

        private void SetArchiveTypeUserInput(string archiveTypeSelected)
        {
            _log.Information($"User action: Select archive type {archiveTypeSelected}");

            ArchiveType tempArchiveType;
            if (ArchiveType.TryParse(archiveTypeSelected, true, out tempArchiveType))
            {
                _isArchiveTypeSelected = true;
                ArchiveType = tempArchiveType;
            }
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
        }

        private void OpenArchiveFolderDialog()
        {
            _log.Information("User action: Open archive folder dialog");

            ArchiveFileName = OpenFolderDialog();

            if (ArchiveFileName == null)
                return;

            _log.Information("User action: Choose archive folder {ArchiveFileName}", ArchiveFileName);

            PresentChosenArchiveInGui(ArchiveFileName, true);
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