using System.Diagnostics;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class LoadArchiveExtractionViewModel : BindableBase
    {
        private readonly ArchiveExtractionReader _archiveExtractionReader;
        private readonly TestEngine _testEngine;
        private readonly IRegionManager _regionManager;
        private string _archiveFileName;

        private string _metadataFileName;
        private bool _isRunningTests;

        public LoadArchiveExtractionViewModel(ArchiveExtractionReader archiveExtractionReader, TestEngine testEngine, IRegionManager regionManager)
        {
            _archiveExtractionReader = archiveExtractionReader;
            _testEngine = testEngine;
            _regionManager = regionManager;
            OpenMetadataFileCommand = new DelegateCommand(OpenMetadataFileDialog);
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()), CanRunTests);

            NavigateCommand = new DelegateCommand(Navigate);
        }

        public string MetadataFileName
        {
            get { return _metadataFileName; }
            set
            {
                SetProperty(ref _metadataFileName, value);
                RunTestEngineCommand.RaiseCanExecuteChanged();
            }
        }

        public string ArchiveFileName
        {
            get { return _archiveFileName; }
            set
            {
                SetProperty(ref _archiveFileName, value);
                RunTestEngineCommand.RaiseCanExecuteChanged();
            }
        }

        private void Navigate()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("archiveFileName", ArchiveFileName);
            navigationParameters.Add("metadataFileName", MetadataFileName);
            _regionManager.RequestNavigate("MainContentRegion", "TestSummary", navigationParameters);
        }

        public DelegateCommand NavigateCommand { get; set; }

        public DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand OpenMetadataFileCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }

        private bool CanRunTests()
        {
            return !_isRunningTests && !string.IsNullOrEmpty(_archiveFileName) && !string.IsNullOrEmpty(_metadataFileName);
        }

        private void RunTests()
        {
            Debug.Print("Issued the RunTests command");
            _isRunningTests = true;
            RunTestEngineCommand.RaiseCanExecuteChanged();


            Archive archive = _archiveExtractionReader.ReadFromFile(ArchiveFileName, MetadataFileName);

            Debug.Print(archive.Uuid);
            Debug.Print(archive.ArchiveType.ToString());
            Debug.Print(archive.WorkingDirectory);

            _testEngine.RunTestsOnArchive(archive);

            _isRunningTests = false;
            RunTestEngineCommand.RaiseCanExecuteChanged();
        }

        private void OpenMetadataFileDialog()
        {
            MetadataFileName = OpenFileDialog();
        }

        private void OpenArchiveFileDialog()
        {
            ArchiveFileName = OpenFileDialog();
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
    }
}