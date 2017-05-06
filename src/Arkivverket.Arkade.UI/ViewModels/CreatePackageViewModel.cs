using System.IO;
using System.Reflection;
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
        private readonly ArkadeApi _arkadeApi;
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _isRunningCreatePackage;
        private bool _selectedPackageTypeAip;
        private bool _selectedPackageTypeSip = true;
        private string _statusMessageText;
        private string _statusMessagePath;
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

        public string StatusMessageText
        {
            get { return _statusMessageText; }
            set { SetProperty(ref _statusMessageText, value); }
        }
        public string StatusMessagePath
        {
            get { return _statusMessagePath; }
            set { SetProperty(ref _statusMessagePath, value); }
        }

        public CreatePackageViewModel(ArkadeApi arkadeApi, IRegionManager regionManager)
        {
            _arkadeApi = arkadeApi;
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
            
            PackageType packageType = SelectedPackageTypeSip ?  PackageType.SubmissionInformationPackage : PackageType.ArchivalInformationPackage;

            // todo must be async
            _arkadeApi.CreatePackage(_testSession, packageType);

            string informationPackageFileName = _testSession.Archive.GetInformationPackageFileName().FullName;
            StatusMessageText = "IP og metadata lagret i ";
            StatusMessagePath = informationPackageFileName;
            Log.Debug("Package created in " + informationPackageFileName);

            _isRunningCreatePackage = false;
            CreatePackageCommand.RaiseCanExecuteChanged();
        }

    }
}