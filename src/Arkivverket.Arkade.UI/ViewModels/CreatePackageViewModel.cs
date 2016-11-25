using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class CreatePackageViewModel : BindableBase, INavigationAware
    {
        private ILogger _log = Log.ForContext<CreatePackageViewModel>();
        private bool _isRunningCreatePackage;
        private bool _selectedPackageTypeAip;
        private bool _selectedPackageTypeSip = true;
        private string _statusMessage;
        private TestSession _testSession;
        private readonly IRegionManager _regionManager;


        public DelegateCommand CreatePackageCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }

        public bool SelectedPackageTypeSip
        {
            get { return _selectedPackageTypeSip; }
            set
            {
                SetProperty(ref _selectedPackageTypeSip, value);
                CreatePackageCommand.RaiseCanExecuteChanged();
            }
        }

        public bool SelectedPackageTypeAip
        {
            get { return _selectedPackageTypeAip; }
            set
            {
                SetProperty(ref _selectedPackageTypeAip, value);
                CreatePackageCommand.RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        public CreatePackageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            CreatePackageCommand = new DelegateCommand(RunCreatePackage, CanExecuteCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(RunNavigateToLoadArchivePage, CanExecuteCreatePackage);
        }


        public void OnNavigatedTo(NavigationContext context)
        {
            _testSession = (TestSession) context.Parameters["TestSession"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        private void RunNavigateToLoadArchivePage()
        {
            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }



        private bool CanExecuteCreatePackage()
        {
            return !_isRunningCreatePackage && (SelectedPackageTypeSip || SelectedPackageTypeAip);
        }

        private void RunCreatePackage()
        {
            Log.Debug("Running create package command");
            _isRunningCreatePackage = true;
            CreatePackageCommand.RaiseCanExecuteChanged();
            
            DirectoryInfo directoryName = GetIpDirectory();

            // todo must be async
            var arkade = new Core.Arkade();
            bool saved = arkade.SaveIp(_testSession, directoryName);
            if (saved)
            {
                StatusMessage = "IP og metadata lagret i " + directoryName;
                Log.Debug("Package created in " + directoryName);
            }

            _isRunningCreatePackage = false;
            CreatePackageCommand.RaiseCanExecuteChanged();
        }

        private DirectoryInfo GetIpDirectory()
        {
            string directoryName = Path.Combine(ArkadeConstants.GetArkadeIpDirectory().FullName, _testSession.Archive.Uuid.GetValue());
            var directoryInfo = new DirectoryInfo(directoryName);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;

            /* If we want to use a FolderBrowserDialog
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = directoryName;
            dialog.ShowNewFolderButton = true;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                directoryName = dialog.SelectedPath;
                return new DirectoryInfo(directoryName);
            }
            else
            {
                return null;
            }
            */
        }
    }
}