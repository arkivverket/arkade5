using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.GUI.Models;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.Core.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arkivverket.Arkade.GUI.Resources;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Taskbar;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class CreatePackageViewModel : BindableBase, INavigationAware
    {
        private readonly ArkadeApi _arkadeApi;
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _isRunningCreatePackage;
        private bool _selectedPackageTypeAip;
        private bool _selectedPackageTypeSip = true;
        private bool _standardLabelIsSelected = true;
        private bool _userdefinedLabelIsSelected;
        private Visibility _progressBarVisibility = Visibility.Hidden;
        private string _statusMessageText;
        private string _statusMessagePath;
        private TestSession _testSession;
        private string _archiveFileName;
        private readonly IRegionManager _regionManager;


        private GuiMetaDataModel _metaDataArchiveDescription = new GuiMetaDataModel(string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveCreators = new ObservableCollectionEx<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataTransferer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataProducer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataOwners = new ObservableCollectionEx<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataCreator = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataRecipient = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, GuiObjectType.system);
        private GuiMetaDataModel _metaDataArchiveSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, GuiObjectType.system);
        private GuiMetaDataModel _metaDataCreatorSoftwareSystem = new GuiMetaDataModel(null, null, null, null, GuiObjectType.system);
        private GuiMetaDataModel _metaDataNoarkSection = new GuiMetaDataModel(null, null, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataExtractionDate = new GuiMetaDataModel(null);


        private IList<String> _systemTypeList  = new List<string>()
        {
            "Noark3", "Noark4", "Noark5", "Fagsystem"
        }; 

        public string ArkadeNameAndCurrentVersion { get; } = $"Arkade 5 {ArkadeVersion.Current}";

        public DelegateCommand CreatePackageCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }
        public DelegateCommand AddMetadataAchiveCreatorEntry { get; set; }
        public DelegateCommand AddMetadataAchiveOwnerEntry { get; set; }
        public DelegateCommand LoadExternalMetadataCommand { get; set; }

        //---------------------------------------------------------------------------
        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                SetProperty(ref _progressBarVisibility, value);
            }
        }


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

        public bool StandardLabelIsSelected
        {
            get { return _standardLabelIsSelected; }
            set { SetProperty(ref _standardLabelIsSelected, value); }
        }

        public bool UserdefinedLabelIsSelected
        {
            get { return _userdefinedLabelIsSelected; }
            set { SetProperty(ref _userdefinedLabelIsSelected, value); }
        }

        public GuiMetaDataModel MetaDataModelArchiveDescription
        {
            get { return _metaDataArchiveDescription; }
            set { SetProperty(ref _metaDataArchiveDescription, value); }
        }


        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveCreators
        {
            get {return _metaDataArchiveCreators; }
            set {SetProperty(ref _metaDataArchiveCreators, value); }
        }



        public GuiMetaDataModel MetaDataTransferer
        {
            get { return _metaDataTransferer; }
            set { SetProperty(ref _metaDataTransferer, value); }
        }

        public GuiMetaDataModel MetaDataProducer
        {
            get { return _metaDataProducer; }
            set { SetProperty(ref _metaDataProducer, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataOwners
        {
            get { return _metaDataOwners; }
            set { SetProperty(ref _metaDataOwners, value); }
        }

        public GuiMetaDataModel MetaDataCreator
        {
            get { return _metaDataCreator; }
            set { SetProperty(ref _metaDataCreator, value); }
        }

        public GuiMetaDataModel MetaDataRecipient
        {
            get { return _metaDataRecipient; }
            set { SetProperty(ref _metaDataRecipient, value); }
        }
        public GuiMetaDataModel MetaDataSystem
        {
            get { return _metaDataSystem; }
            set { SetProperty(ref _metaDataSystem, value); }
        }

        public GuiMetaDataModel MetaDataArchiveSystem
        {
            get { return _metaDataArchiveSystem; }
            set { SetProperty(ref _metaDataArchiveSystem, value); }
        }

        public GuiMetaDataModel MetaDataCreatorSoftwareSystem
        {
            get { return _metaDataCreatorSoftwareSystem; }
            set { SetProperty(ref _metaDataCreatorSoftwareSystem, value); }
        }

        public GuiMetaDataModel MetaDataNoarkSection
        {
            get { return _metaDataNoarkSection; }
            set { SetProperty(ref _metaDataNoarkSection, value); }
        }

        public GuiMetaDataModel MetaDataExtractionDate
        {
            get { return _metaDataExtractionDate; }
            set { SetProperty(ref _metaDataExtractionDate, value); }
        }

        public IList<String> SystemTypeList
        {
            get { return _systemTypeList; }
            set { SetProperty(ref _systemTypeList, value); }
        }


        //---------------------------------------------------------------------------


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

            LoadExternalMetadataCommand = new DelegateCommand(RunLoadExternalMetadata, CanLoadMetadata);
            CreatePackageCommand = new DelegateCommand(RunCreatePackage, CanExecuteCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(RunNavigateToLoadArchivePage, CanLeaveCreatePackageView);
            AddMetadataAchiveCreatorEntry = new DelegateCommand(RunAddMetadataAchiveCreatorEntry);
            AddMetadataAchiveOwnerEntry = new DelegateCommand(RunAddMetadataAchiveOwnerEntry);

           ((INotifyPropertyChanged)MetaDataArchiveCreators).PropertyChanged += (x, y) => OnMetaDataArchiveCreatorsDataElementChaneChange();
           ((INotifyPropertyChanged)MetaDataOwners).PropertyChanged += (x, y) => OnMetaDataOwnersDataElementChaneChange();

        }

        public void OnMetaDataArchiveCreatorsDataElementChaneChange()
        {
            // Fires when any change is carried out in the MetaDataArchiveCreators ObservableCollection
            // Calls function to aminister GUI based as needed
            _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(MetaDataArchiveCreators);
        }

        public void OnMetaDataOwnersDataElementChaneChange()
        {
            // Fires when any change is carried out in the MetaDataArchiveCreators ObservableCollection
            // Calls function to aminister GUI based as needed
            _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(MetaDataOwners);
        }



        public void RunAddMetadataAchiveCreatorEntry()
        {
            MetaDataArchiveCreators.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveOwnerEntry()
        {
            MetaDataOwners.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        }


        public void OnNavigatedTo(NavigationContext context)
        {
            try
            {
                _testSession = (TestSession) context.Parameters["TestSession"];
                _archiveFileName = (string) context.Parameters["archiveFileName"];

                FileInfo includedMetadataFile =
                    _testSession.Archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

                LoadMetadataIntoForm(includedMetadataFile,
                    delegate { Log.Error("Not able to load metadata from file: " + includedMetadataFile.FullName); }
                );

                // Pre populate metadata objects that require at least one entry
                RunAddMetadataAchiveCreatorEntry();
                RunAddMetadataAchiveOwnerEntry();
            }
            catch (Exception e)
            {
                
                string message = string.Format(Resources.GUI.ErrorGeneral, e.Message);
                StatusMessageText = message;
                
                Log.Error(e, message);
            }
        }

        private void LoadMetadataIntoForm(FileSystemInfo metadataFile, Action errorAction)
        {
            try
            {
                ArchiveMetadata metadata = MetadataLoader.Load(metadataFile.FullName);

                FillForm(metadata);
            }
            catch
            {
                errorAction.Invoke();
            }
        }

        private void FillForm(ArchiveMetadata archiveMetadata)
        {
            if (archiveMetadata.AgreementNumber != null) // archiveMetadata.ArchiveDescription is not required
                MetaDataModelArchiveDescription = GuiMetadataMapper.MapToArchiveDescription(
                    archiveMetadata.ArchiveDescription, archiveMetadata.AgreementNumber
                );

            if (archiveMetadata.ArchiveCreators != null && archiveMetadata.ArchiveCreators.Any())
                MetaDataArchiveCreators = GuiMetadataMapper.MapToArchiveCreators(archiveMetadata.ArchiveCreators);

            if (archiveMetadata.Transferer != null)
                MetaDataTransferer = GuiMetadataMapper.MapToTransferer(archiveMetadata.Transferer);

            if (archiveMetadata.Producer != null)
                MetaDataProducer = GuiMetadataMapper.MapToProducer(archiveMetadata.Producer);

            if (archiveMetadata.Owners != null && archiveMetadata.Owners.Any())
                MetaDataOwners = GuiMetadataMapper.MapToOwners(archiveMetadata.Owners);

            if (archiveMetadata.Creator != null)
                MetaDataCreator = GuiMetadataMapper.MapToCreator(archiveMetadata.Creator);

            if (archiveMetadata.Recipient != null)
                MetaDataRecipient = GuiMetadataMapper.MapToRecipient(archiveMetadata.Recipient);

            if (archiveMetadata.System != null)
                MetaDataSystem = GuiMetadataMapper.MapToSystem(archiveMetadata.System);

            if (archiveMetadata.ArchiveSystem != null)
                MetaDataArchiveSystem = GuiMetadataMapper.MapToArchiveSystem(archiveMetadata.ArchiveSystem);

            if (archiveMetadata.StartDate != null)
                MetaDataNoarkSection.StartDate = archiveMetadata.StartDate;

            if (archiveMetadata.EndDate != null)
                MetaDataNoarkSection.EndDate = archiveMetadata.EndDate;

            if (archiveMetadata.ExtractionDate != null)
                MetaDataExtractionDate = GuiMetadataMapper.MapToExtractionDate(archiveMetadata.ExtractionDate);

            if (archiveMetadata.Label != null)
                MetaDataNoarkSection.UserdefinedLabel = archiveMetadata.Label;

            if (archiveMetadata.CreatorSoftwareSystem != null)
                MetaDataCreatorSoftwareSystem = GuiMetadataMapper.MapToCreatorSoftwareSystem(archiveMetadata.CreatorSoftwareSystem);

            UserdefinedLabelIsSelected = true;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        private void RunNavigateToLoadArchivePage()
        {
            Log.Information("User action: Leave test session and return to load archive window");

            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }

        private bool CanLeaveCreatePackageView()
        {
            return !_isRunningCreatePackage;
        }

        private bool CanExecuteCreatePackage()
        {
            return !_isRunningCreatePackage && (SelectedPackageTypeSip || SelectedPackageTypeAip);
        }

        private bool CanLoadMetadata()
        {
            return !_isRunningCreatePackage;
        }

        private void RunLoadExternalMetadata()
        {
            Log.Information("User action: Open metadata file");

            string suggestedMetadataFileDirectory = new FileInfo(_archiveFileName).DirectoryName;

            var selectMetadataFileDialog = new CommonOpenFileDialog
            {
                Title = MetaDataGUI.SelectMetadataFile,
                InitialDirectory = suggestedMetadataFileDirectory,
            };

            string metadataFileName;

            if (selectMetadataFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                metadataFileName = selectMetadataFileDialog.FileName;
            else return;

            Log.Information("User action: Choose metadata file", metadataFileName);

            var metadataFile = new FileInfo(metadataFileName);

            LoadMetadataIntoForm(metadataFile,
                delegate
                {
                    MessageBox.Show(
                        string.Format(MetaDataGUI.MetadataLoadError, metadataFile.FullName),
                        null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                    );
                }
            );
        }

        private void RunCreatePackage()
        {
            Log.Information("User action: Create package");

            string suggestedOutputDirectory = new FileInfo(_archiveFileName).DirectoryName;

            var selectOutputDirectoryDialog = new CommonOpenFileDialog
            {
                Title = Resources.MetaDataGUI.SelectOutputDirectoryMessage,
                IsFolderPicker = true,
                InitialDirectory = suggestedOutputDirectory,
                DefaultFileName = suggestedOutputDirectory,
            };
            
            string outputDirectory;

            if (selectOutputDirectoryDialog.ShowDialog() == CommonFileDialogResult.Ok)
                outputDirectory = selectOutputDirectoryDialog.FileName;
            else return;
            
            ProgressBarVisibility = Visibility.Visible;
            Log.Information("User action: Choose package destination {informationPackageDestination}", outputDirectory);
            
            _testSession.ArchiveMetadata = new ArchiveMetadata
            {
                Id = $"UUID:{_testSession.Archive.Uuid}",

                Label = ArchiveMetadataMapper.MapToLabel(_metaDataNoarkSection, StandardLabelIsSelected),
                ArchiveDescription = ArchiveMetadataMapper.MapToArchiveDescription(_metaDataArchiveDescription),
                AgreementNumber = ArchiveMetadataMapper.MapToAgreementNumber(_metaDataArchiveDescription),
                ArchiveCreators = ArchiveMetadataMapper.MapToArchiveCreators(_metaDataArchiveCreators.Where(c => !c.IsDeleted)),
                Transferer = ArchiveMetadataMapper.MapToTransferer(_metaDataTransferer),
                Producer = ArchiveMetadataMapper.MapToProducer(_metaDataProducer),
                Owners = ArchiveMetadataMapper.MapToArchiveOwners(_metaDataOwners.Where(o => !o.IsDeleted)),
                Creator = ArchiveMetadataMapper.MapToCreator(_metaDataCreator),
                Recipient = ArchiveMetadataMapper.MapToRecipient(_metaDataRecipient),
                System = ArchiveMetadataMapper.MapToSystem(_metaDataSystem),
                ArchiveSystem = ArchiveMetadataMapper.MapToArchiveSystem(_metaDataArchiveSystem),
                StartDate = ArchiveMetadataMapper.MapToStartDate(_metaDataNoarkSection),
                EndDate = ArchiveMetadataMapper.MapToEndDate(_metaDataNoarkSection),
                ExtractionDate = ArchiveMetadataMapper.MapToExtractionDate(_metaDataExtractionDate),
                PackageType = ArchiveMetadataMapper.MapToPackageType(SelectedPackageTypeSip)
            };

            _isRunningCreatePackage = true;
            CreatePackageCommand.RaiseCanExecuteChanged();
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            Task.Factory.StartNew(() => CreatePackageRunEngine(outputDirectory)).ContinueWith(t => OnCompletedCreatePackage());
        }

        private void OnCompletedCreatePackage()
        {
            _isRunningCreatePackage = false;
            CreatePackageCommand.RaiseCanExecuteChanged();
            TaskbarManager.Instance.SetProgressValue(1, 1);
        }


        private void CreatePackageRunEngine(string outputDirectory)
        {
            try
            {
                string packageFilePath = _arkadeApi.CreatePackage(_testSession, outputDirectory);

                string packageOutputContainer = new FileInfo(packageFilePath).DirectoryName;

                string argument = "/select, \"" + packageOutputContainer + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);

                StatusMessageText = "IP og metadata lagret i ";
                StatusMessagePath = packageOutputContainer;
                Log.Debug("Package created in " + packageOutputContainer);

            }
            catch (Exception exception)
            {
                if (exception is IOException)
                {
                    StatusMessageText = Resources.MetaDataGUI.PackageCreationErrorStatusMessage;
                    Log.Debug(Resources.MetaDataGUI.PackageCreationErrorLogMessage);
                }

                if (exception is InsufficientDiskSpaceException)
                {
                    StatusMessageText = MetaDataGUI.UnsufficientDiskSpaceStatusMessage;
                    Log.Debug(MetaDataGUI.UnsufficientDiskSpaceLogMessage);
                }

                string fileName = new DetailedExceptionMessage(exception).WriteToFile();

                if (!string.IsNullOrEmpty(fileName))
                    StatusMessagePath = string.Format(Resources.GUI.DetailedErrorMessageInfo, fileName);

                _isRunningCreatePackage = false;
            }
            ProgressBarVisibility = Visibility.Hidden;
        }




        private int _GetNumNotDeletedEntriesIn(ObservableCollection<GuiMetaDataModel> collection)
        {
            return collection.Count(x => x.IsDeleted == false);
        }

        private void _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(ObservableCollection<GuiMetaDataModel> collection)
        {
            if (_GetNumNotDeletedEntriesIn(collection) == 1)
            {
                foreach (var entry in collection)
                {
                    if (entry.IsDeleted == false)
                        entry.SetDeleteButtonHidden();
                }
            }
            else
            {
                foreach (var entry in collection)
                {
                    if (entry.IsDeleted == false)
                        entry.SetDeleteButtonVisible();
                }
            }
        }

    }
}
