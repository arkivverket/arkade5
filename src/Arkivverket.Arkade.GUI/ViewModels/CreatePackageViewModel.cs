using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.GUI.Models;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.Core.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Views;
using MessageBox = System.Windows.MessageBox;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class CreatePackageViewModel : BindableBase, INavigationAware
    {
        private readonly ArkadeApi _arkadeApi;
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _generateFileFormatInfoSelected;
        private bool _selectedPackageTypeAip;
        private bool _selectedPackageTypeSip = true;
        private bool _standardLabelIsSelected = true;
        private bool _userdefinedLabelIsSelected;
        private Visibility _progressBarVisibility = Visibility.Hidden;
        private string _statusMessageText;
        private string _statusMessagePath;
        private string _includeFormatInfoFile;
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

        private bool IsRunningCreatePackage()
        {
            return ArkadeProcessingState.PackingIsStarted && !ArkadeProcessingState.PackingIsFinished;
        }

        private IList<String> _systemTypeList  = new List<string>()
        {
            "Noark3", "Noark5", "Fagsystem", "Siard"
        }; 

        public string ArkadeNameAndCurrentVersion { get; } = $"Arkade 5 {ArkadeVersion.Current}";

        public DelegateCommand CreatePackageCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }
        public DelegateCommand AddMetadataArchiveCreatorEntry { get; set; }
        public DelegateCommand AddMetadataArchiveOwnerEntry { get; set; }
        public DelegateCommand LoadExternalMetadataCommand { get; set; }

        //---------------------------------------------------------------------------
        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            set => SetProperty(ref _progressBarVisibility, value);
        }

        public bool GenerateFileFormatInfoSelected
        {
            get => _generateFileFormatInfoSelected;
            set => SetProperty(ref _generateFileFormatInfoSelected, value);
        }
        public bool SelectedPackageTypeSip
        {
            get => _selectedPackageTypeSip;
            set
            {
                SetProperty(ref _selectedPackageTypeSip, value);
                CreatePackageCommand.RaiseCanExecuteChanged();
            }
        }

        public bool SelectedPackageTypeAip
        {
            get => _selectedPackageTypeAip;
            set
            {
                SetProperty(ref _selectedPackageTypeAip, value);
                CreatePackageCommand.RaiseCanExecuteChanged();
            }
        }

        public bool StandardLabelIsSelected
        {
            get => _standardLabelIsSelected;
            set => SetProperty(ref _standardLabelIsSelected, value);
        }

        public bool UserdefinedLabelIsSelected
        {
            get => _userdefinedLabelIsSelected;
            set => SetProperty(ref _userdefinedLabelIsSelected, value);
        }

        public GuiMetaDataModel MetaDataModelArchiveDescription
        {
            get => _metaDataArchiveDescription;
            set => SetProperty(ref _metaDataArchiveDescription, value);
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveCreators
        {
            get => _metaDataArchiveCreators;
            set => SetProperty(ref _metaDataArchiveCreators, value);
        }

        public GuiMetaDataModel MetaDataTransferer
        {
            get => _metaDataTransferer;
            set => SetProperty(ref _metaDataTransferer, value);
        }

        public GuiMetaDataModel MetaDataProducer
        {
            get => _metaDataProducer;
            set => SetProperty(ref _metaDataProducer, value);
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataOwners
        {
            get => _metaDataOwners;
            set => SetProperty(ref _metaDataOwners, value);
        }

        public GuiMetaDataModel MetaDataCreator
        {
            get => _metaDataCreator;
            set => SetProperty(ref _metaDataCreator, value);
        }

        public GuiMetaDataModel MetaDataRecipient
        {
            get => _metaDataRecipient;
            set => SetProperty(ref _metaDataRecipient, value);
        }
        public GuiMetaDataModel MetaDataSystem
        {
            get => _metaDataSystem;
            set => SetProperty(ref _metaDataSystem, value);
        }

        public GuiMetaDataModel MetaDataArchiveSystem
        {
            get => _metaDataArchiveSystem;
            set => SetProperty(ref _metaDataArchiveSystem, value);
        }

        public GuiMetaDataModel MetaDataCreatorSoftwareSystem
        {
            get => _metaDataCreatorSoftwareSystem;
            set => SetProperty(ref _metaDataCreatorSoftwareSystem, value);
        }

        public GuiMetaDataModel MetaDataNoarkSection
        {
            get => _metaDataNoarkSection;
            set => SetProperty(ref _metaDataNoarkSection, value);
        }

        public GuiMetaDataModel MetaDataExtractionDate
        {
            get => _metaDataExtractionDate;
            set => SetProperty(ref _metaDataExtractionDate, value);
        }

        public IList<String> SystemTypeList
        {
            get => _systemTypeList;
            set => SetProperty(ref _systemTypeList, value);
        }


        //---------------------------------------------------------------------------


        public string StatusMessageText
        {
            get => _statusMessageText;
            set => SetProperty(ref _statusMessageText, value);
        }

        public string StatusMessagePath
        {
            get => _statusMessagePath;
            set => SetProperty(ref _statusMessagePath, value);
        }

        public string IncludeFormatInfoFile
        {
            get => _includeFormatInfoFile;
            set => SetProperty(ref _includeFormatInfoFile, value);
        }


        public CreatePackageViewModel(ArkadeApi arkadeApi, IRegionManager regionManager)
        {
            _arkadeApi = arkadeApi;
            _regionManager = regionManager;

            LoadExternalMetadataCommand = new DelegateCommand(RunLoadExternalMetadata, CanLoadMetadata);
            CreatePackageCommand = new DelegateCommand(RunCreatePackage, CanExecuteCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(RunNavigateToLoadArchivePage, CanLeaveCreatePackageView);
            AddMetadataArchiveCreatorEntry = new DelegateCommand(RunAddMetadataArchiveCreatorEntry);
            AddMetadataArchiveOwnerEntry = new DelegateCommand(RunAddMetadataArchiveOwnerEntry);

           ((INotifyPropertyChanged)MetaDataArchiveCreators).PropertyChanged += (x, y) => OnMetaDataArchiveCreatorsDataElementChange();
           ((INotifyPropertyChanged)MetaDataOwners).PropertyChanged += (x, y) => OnMetaDataOwnersDataElementChange();

        }

        public void OnMetaDataArchiveCreatorsDataElementChange()
        {
            // Fires when any change is carried out in the MetaDataArchiveCreators ObservableCollection
            // Calls function to administer GUI based as needed
            _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(MetaDataArchiveCreators);
        }

        public void OnMetaDataOwnersDataElementChange()
        {
            // Fires when any change is carried out in the MetaDataArchiveCreators ObservableCollection
            // Calls function to administer GUI based as needed
            _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(MetaDataOwners);
        }



        public void RunAddMetadataArchiveCreatorEntry()
        {
            MetaDataArchiveCreators.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataArchiveOwnerEntry()
        {
            MetaDataOwners.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
        }


        public void OnNavigatedTo(NavigationContext context)
        {
            try
            {
                _testSession = (TestSession) context.Parameters["TestSession"];
                _archiveFileName = (string) context.Parameters["archiveFileName"];

                if (_testSession.Archive.ArchiveType == ArchiveType.Siard)
                    IncludeFormatInfoFile = MetaDataGUI.CreateLobFormatInfoFileText;
                else
                    IncludeFormatInfoFile = MetaDataGUI.CreateDocumentFileInfoText;

                FileInfo includedMetadataFile =
                    _testSession.Archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

                LoadMetadataIntoForm(includedMetadataFile,
                    delegate { Log.Error("Not able to load metadata from file: " + includedMetadataFile.FullName); }
                );

                // Pre populate metadata objects that require at least one entry
                RunAddMetadataArchiveCreatorEntry();
                RunAddMetadataArchiveOwnerEntry();
            }
            catch (Exception e)
            {
                
                string message = string.Format(Languages.GUI.ErrorGeneral, e.Message);
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
            MainWindow.ProgressBarWorker.ReportProgress(0, "reset");
        }

        private void RunNavigateToLoadArchivePage()
        {
            Log.Information("User action: Leave test session and return to load archive window");

            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }

        private bool CanLeaveCreatePackageView()
        {
            return !IsRunningCreatePackage();
        }

        private bool CanExecuteCreatePackage()
        {
            return !IsRunningCreatePackage() && (SelectedPackageTypeSip || SelectedPackageTypeAip);
        }

        private bool CanLoadMetadata()
        {
            return !IsRunningCreatePackage();
        }

        private void RunLoadExternalMetadata()
        {
            Log.Information("User action: Open metadata file");

            string suggestedMetadataFileDirectory = new FileInfo(_archiveFileName).DirectoryName;

            var selectMetadataFileDialog = new OpenFileDialog
            {
                Title = MetaDataGUI.SelectMetadataFile,
                InitialDirectory = suggestedMetadataFileDirectory,
            };

            string metadataFileName;

            if (selectMetadataFileDialog.ShowDialog() == DialogResult.OK)
                metadataFileName = selectMetadataFileDialog.FileName;
            else return;

            Log.Information("User action: Choose metadata file", metadataFileName);

            var metadataFile = new FileInfo(metadataFileName);

            LoadMetadataIntoForm(metadataFile,
                delegate
                {
                    MessageBox.Show(
                        string.Format(MetaDataGUI.MetadataLoadError, metadataFile.FullName),
                        null, MessageBoxButton.OK, MessageBoxImage.Exclamation
                    );
                }
            );
        }

        private void RunCreatePackage()
        {
            Log.Information("User action: Create package");

            var selectOutputDirectoryDialog = new FolderBrowserDialog
            {
                Description = MetaDataGUI.SelectOutputDirectoryMessage,
                UseDescriptionForTitle = true,
            };
            
            string outputDirectory;

            if (selectOutputDirectoryDialog.ShowDialog() == DialogResult.OK)
                outputDirectory = selectOutputDirectoryDialog.SelectedPath;
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

            _testSession.GenerateFileFormatInfo = GenerateFileFormatInfoSelected;

            ArkadeProcessingState.PackingIsStarted = true;
            MainWindowViewModel.ShowSettingsCommand.RaiseCanExecuteChanged();
            
            CreatePackageCommand.RaiseCanExecuteChanged();
            MainWindow.ProgressBarWorker.ReportProgress(0);

            Task.Factory.StartNew(() => CreatePackageRunEngine(outputDirectory)).ContinueWith(t => OnCompletedCreatePackage());
        }

        private void OnCompletedCreatePackage()
        {
            ArkadeProcessingState.PackingIsFinished = true;
            CreatePackageCommand.RaiseCanExecuteChanged();
            MainWindow.ProgressBarWorker.ReportProgress(100);
        }


        private void CreatePackageRunEngine(string outputDirectory)
        {
            try
            {
                string packageFilePath = _arkadeApi.CreatePackage(_testSession, outputDirectory);

                string packageOutputContainer = new FileInfo(packageFilePath).DirectoryName;

                string argument = "/select, \"" + packageOutputContainer + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);

                StatusMessageText = CreatePackageGUI.IPandMetadataSuccessfullyCreatedStatusMessage;
                StatusMessagePath = packageOutputContainer;
                Log.Debug("Package created at " + packageOutputContainer);

            }
            catch (Exception exception)
            {
                if (exception is IOException)
                {
                    StatusMessageText = MetaDataGUI.PackageCreationErrorStatusMessage;
                    Log.Debug("Error: Could not create/overwrite package");
                }

                if (exception is InsufficientDiskSpaceException)
                {
                    StatusMessageText = MetaDataGUI.UnsufficientDiskSpaceStatusMessage;
                    Log.Debug("Not enough disk space on target location");
                }

                string fileName = new DetailedExceptionMessage(exception).WriteToFile();

                if (!string.IsNullOrEmpty(fileName))
                    StatusMessagePath = string.Format(Languages.GUI.DetailedErrorMessageInfo, fileName);

                ArkadeProcessingState.PackingIsFinished = true;
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
