using System.Diagnostics;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class LoadArchiveExtractionViewModel : BindableBase
    {
        private readonly TestEngine _testEngine;
        private string _archiveFileName;

        private string _metadataFileName;


        public LoadArchiveExtractionViewModel(TestEngine testEngine)
        {
            _testEngine = testEngine;
            OpenMetadataFileCommand = new DelegateCommand(OpenMetadataFileDialog);
            OpenArchiveFileCommand = new DelegateCommand(OpenArchiveFileDialog);
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));
        }

        public string MetadataFileName
        {
            get { return _metadataFileName; }
            set { SetProperty(ref _metadataFileName, value); }
        }

        public string ArchiveFileName
        {
            get { return _archiveFileName; }
            set { SetProperty(ref _archiveFileName, value); }
        }

        public DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand OpenMetadataFileCommand { get; set; }
        public DelegateCommand OpenArchiveFileCommand { get; set; }

        private void RunTests()
        {
            Debug.Print("Issued the RunTests command");
            var workingDirectory = @"C:\temp\n5-alice-liten";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;

            //      _testEngine.RunTestsOnArchive(archiveExtraction);
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