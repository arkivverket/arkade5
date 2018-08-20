using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Forms;
using Arkivverket.Arkade.GUI.Resources;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Arkivverket.Arkade.GUI.ViewModels
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
        private string _archiveFileName;
        private readonly IRegionManager _regionManager;

        private readonly PopulateMetadataDataModels _populateMetadataDataModels = new PopulateMetadataDataModels();

        private GuiMetaDataModel _metaDataArchiveDescription = new GuiMetaDataModel(string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveCreators = new ObservableCollectionEx<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataTransferer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataProducer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataOwners = new ObservableCollectionEx<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataRecipient = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, GuiObjectType.system);
        private GuiMetaDataModel _metaDataArchiveSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, GuiObjectType.system);
        private ObservableCollection<GuiMetaDataModel> _metaDataComments = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataHistory = new GuiMetaDataModel(string.Empty, GuiObjectType.history);
        private GuiMetaDataModel _metaDataNoarkSection = new GuiMetaDataModel(null, null, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataExtractionDate = new GuiMetaDataModel(null);


        private ObservableCollection<GuiMetaDataModel> _metadataPreregistreredUsers = new ObservableCollection<GuiMetaDataModel>();
        private IList<String> _systemTypeList  = new List<string>()
        {
            "Noark3", "Noark4", "Noark5", "Fagsystem"
        }; 


        private GuiMetaDataModel _selectedCreatorDataModel;
        private GuiMetaDataModel _selectedTransfererDataModel;
        private GuiMetaDataModel _selectedProducerDataModel;
        private GuiMetaDataModel _selectedOwnerDataModel;
        private GuiMetaDataModel _selectedArchiveSystemDataModel;



        private readonly List<MetadataEntityInformationUnit> _metaDataEntityInformationUnits = new List<MetadataEntityInformationUnit>();

        public DelegateCommand CreatePackageCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }
        public DelegateCommand AddMetadataAchiveCreatorEntry { get; set; }
        public DelegateCommand AddMetadataAchiveOwnerEntry { get; set; }
        public DelegateCommand AddMetadataCommentEntry { get; set; }
        public DelegateCommand LoadExternalMetadataCommand { get; set; }

        //---------------------------------------------------------------------------


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

        public ObservableCollection<GuiMetaDataModel> MetaDataComments
        {
            get { return _metaDataComments; }
            set { SetProperty(ref _metaDataComments, value); }
        }

        public GuiMetaDataModel MetaDataHistory
        {
            get { return _metaDataHistory; }
            set { SetProperty(ref _metaDataHistory, value); }
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

        public ObservableCollection<GuiMetaDataModel> MetaDataPreregistreredUsers
        {
            get { return _metadataPreregistreredUsers; }
            set { SetProperty(ref _metadataPreregistreredUsers, value); }
        }


        public IList<String> SystemTypeList
        {
            get { return _systemTypeList; }
            set { SetProperty(ref _systemTypeList, value); }
        }


        //---------------------------------------------------------------------------


        public GuiMetaDataModel SelectedTransfererDataModel
        {
            get { return _selectedTransfererDataModel; }
            set
            {
                SetProperty(ref _selectedTransfererDataModel, value);
                MetaDataTransferer.Entity = SelectedTransfererDataModel.Entity;
                MetaDataTransferer.ContactPerson = SelectedTransfererDataModel.ContactPerson;
                MetaDataTransferer.Telephone = SelectedTransfererDataModel.Telephone;
                MetaDataTransferer.Email = SelectedTransfererDataModel.Email;
            }
        }

        public GuiMetaDataModel SelectedProducerDataModel
        {
            get { return _selectedProducerDataModel; }
            set
            {
                SetProperty(ref _selectedProducerDataModel, value);
                MetaDataProducer.Entity = SelectedProducerDataModel.Entity;
                MetaDataProducer.ContactPerson = SelectedProducerDataModel.ContactPerson;
                MetaDataProducer.Telephone = SelectedProducerDataModel.Telephone;
                MetaDataProducer.Email = SelectedProducerDataModel.Email;
            }
        }

        public GuiMetaDataModel SelectedOwnerDataModel
        {
            get { return _selectedOwnerDataModel; }
            set
            {
                SetProperty(ref _selectedOwnerDataModel, value);
                SetProperty(ref _selectedCreatorDataModel, value);
                if (_EnterElementIfOneNotDeletedEntryIsNotFilled(MetaDataOwners, _selectedOwnerDataModel.Entity, _selectedOwnerDataModel.ContactPerson, _selectedOwnerDataModel.Telephone, _selectedOwnerDataModel.Email) == false)
                {
                    MetaDataOwners.Add(new GuiMetaDataModel(_selectedOwnerDataModel.Entity, _selectedOwnerDataModel.ContactPerson, _selectedOwnerDataModel.Telephone, _selectedOwnerDataModel.Email));
                }
            }
        }

        public GuiMetaDataModel SelectedArchiveSystemDataModel
        {
            get { return _selectedOwnerDataModel; }
            set
            {
                SetProperty(ref _selectedArchiveSystemDataModel, value);
                MetaDataArchiveSystem = _selectedArchiveSystemDataModel;
            }
        }

        public GuiMetaDataModel SelectedCreatorDataModel
        {
            get { return _selectedCreatorDataModel; }
            set
            {
                SetProperty(ref _selectedCreatorDataModel, value);
                if (_EnterElementIfOneNotDeletedEntryIsNotFilled(MetaDataArchiveCreators, _selectedCreatorDataModel.Entity, _selectedCreatorDataModel.ContactPerson, _selectedCreatorDataModel.Telephone, _selectedCreatorDataModel.Email) == false)
                {
                    MetaDataArchiveCreators.Add(new GuiMetaDataModel(_selectedCreatorDataModel.Entity, _selectedCreatorDataModel.ContactPerson, _selectedCreatorDataModel.Telephone, _selectedCreatorDataModel.Email));
                }
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

            LoadExternalMetadataCommand = new DelegateCommand(RunLoadExternalMetadata, CanLoadMetadata);
            CreatePackageCommand = new DelegateCommand(RunCreatePackage, CanExecuteCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(RunNavigateToLoadArchivePage, CanLeaveCreatePackageView);
            AddMetadataAchiveCreatorEntry = new DelegateCommand(RunAddMetadataAchiveCreatorEntry);
            AddMetadataAchiveOwnerEntry = new DelegateCommand(RunAddMetadataAchiveOwnerEntry);
            AddMetadataCommentEntry = new DelegateCommand(RunAddMetadataCommentEntry);

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
            MetaDataArchiveCreators.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveOwnerEntry()
        {
            MetaDataOwners.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }
        public void RunAddMetadataCommentEntry()
        {
            MetaDataComments.Add(new GuiMetaDataModel(string.Empty, GuiObjectType.comment));
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

                FileInfo predefinedMetadataFieldValuesFileInfo = GetPredefinedMetadataFieldValuesFileInfo();

                if (predefinedMetadataFieldValuesFileInfo.Exists)
                    LoadPredefinedMetadataFieldValues(predefinedMetadataFieldValuesFileInfo);
                else
                    CreatePredefinedMetadataFieldValuesFile(predefinedMetadataFieldValuesFileInfo);

                _populateMetadataDataModels.DatafillArchiveEntity(_metaDataEntityInformationUnits, MetaDataPreregistreredUsers);

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
                ArchiveMetadata metadata = DiasMetsLoader.Load(metadataFile.FullName);

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

            if (archiveMetadata.Recipient != null)
                MetaDataRecipient = GuiMetadataMapper.MapToRecipient(archiveMetadata.Recipient);

            if (archiveMetadata.System != null)
                MetaDataSystem = GuiMetadataMapper.MapToSystem(archiveMetadata.System);

            if (archiveMetadata.ArchiveSystem != null)
                MetaDataArchiveSystem = GuiMetadataMapper.MapToArchiveSystem(archiveMetadata.ArchiveSystem);

            if (archiveMetadata.Comments != null && archiveMetadata.Comments.Any())
                MetaDataComments = GuiMetadataMapper.MapToComments(archiveMetadata.Comments);

            if (archiveMetadata.StartDate != null)
                MetaDataNoarkSection.StartDate = archiveMetadata.StartDate;

            if (archiveMetadata.EndDate != null)
                MetaDataNoarkSection.EndDate = archiveMetadata.EndDate;

            if (archiveMetadata.ExtractionDate != null)
                MetaDataExtractionDate = GuiMetadataMapper.MapToExtractionDate(archiveMetadata.ExtractionDate);
        }

        private static FileInfo GetPredefinedMetadataFieldValuesFileInfo()
        {
            string predefinedMetadataFieldValuesFileFullName = Path.Combine(
                ArkadeProcessingArea.RootDirectory.FullName,
                ArkadeConstants.MetadataPredefinedFieldValuesFileName
            );

            return new FileInfo(predefinedMetadataFieldValuesFileFullName);
        }

        private void LoadPredefinedMetadataFieldValues(FileInfo predefinedMetadataFieldValuesFileInfo)
        {
            var fileReader = new XmlTextReader(predefinedMetadataFieldValuesFileInfo.FullName) { Namespaces = false };

            while (fileReader.Read())
            {
                if (fileReader.Name.Equals("MetadataEntityInformationUnit") && fileReader.IsStartElement())
                {
                    string infoUnitXml = fileReader.ReadOuterXml();

                    var infoUnit = SerializeUtil.DeserializeFromString<MetadataEntityInformationUnit>(infoUnitXml);

                    _metaDataEntityInformationUnits.Add(infoUnit);
                }
            }

            fileReader.Close();
        }

        private void CreatePredefinedMetadataFieldValuesFile(FileInfo predefinedMetadataFieldValuesFileInfo)
        {
            var metadataEntityInformationUnits = new List<MetadataEntityInformationUnit>
            {
                new MetadataEntityInformationUnit
                {
                    Entity = "Eksempelorganisasjon 1",
                    ContactPerson = "Ola Nordmann",
                    Telephone = "99999999",
                    Email = "ola@nordmann.no"
                },
                new MetadataEntityInformationUnit
                {
                    Entity = "Eksempelorganisasjon 2",
                    ContactPerson = "Kari Nordmann",
                    Telephone = "44444444",
                    Email = "kari@nordmann.no"
                }
            };

            SerializeUtil.SerializeToFile(metadataEntityInformationUnits, predefinedMetadataFieldValuesFileInfo, null);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
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
                DefaultFileName = ArkadeConstants.InfoXmlFileName,
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
            
            Log.Information("User action: Choose package destination {informationPackageDestination}", outputDirectory);
            
            _testSession.ArchiveMetadata = new ArchiveMetadata
            {
                Id = $"UUID:{_testSession.Archive.Uuid}",

                ArchiveDescription = ArchiveMetadataMapper.MapToArchiveDescription(_metaDataArchiveDescription),
                AgreementNumber = ArchiveMetadataMapper.MapToAgreementNumber(_metaDataArchiveDescription),
                ArchiveCreators = ArchiveMetadataMapper.MapToArchiveCreators(_metaDataArchiveCreators.Where(c => !c.IsDeleted)),
                Transferer = ArchiveMetadataMapper.MapToTransferer(_metaDataTransferer),
                Producer = ArchiveMetadataMapper.MapToProducer(_metaDataProducer),
                Owners = ArchiveMetadataMapper.MapToArchiveOwners(_metaDataOwners.Where(o => !o.IsDeleted)),
                Recipient = ArchiveMetadataMapper.MapToRecipient(_metaDataRecipient),
                System = ArchiveMetadataMapper.MapToSystem(_metaDataSystem),
                ArchiveSystem = ArchiveMetadataMapper.MapToArchiveSystem(_metaDataArchiveSystem),
                Comments = ArchiveMetadataMapper.MapToComments(_metaDataComments),
                StartDate = ArchiveMetadataMapper.MapToStartDate(_metaDataNoarkSection),
                EndDate = ArchiveMetadataMapper.MapToEndDate(_metaDataNoarkSection),
                ExtractionDate = ArchiveMetadataMapper.MapToExtractionDate(_metaDataExtractionDate),
                PackageType = ArchiveMetadataMapper.MapToPackageType(SelectedPackageTypeSip)
            };

            _isRunningCreatePackage = true;
            CreatePackageCommand.RaiseCanExecuteChanged();

            // todo must be async

            try
            {
                string packageFilePath = _arkadeApi.CreatePackage(_testSession, outputDirectory);

                string packageOutputContainer = new FileInfo(packageFilePath).DirectoryName;

                string argument = "/select, \"" + packageOutputContainer + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);

                StatusMessageText = "IP og metadata lagret i ";
                StatusMessagePath = packageOutputContainer;
                Log.Debug("Package created in " + packageOutputContainer);

                _isRunningCreatePackage = false;
                //CreatePackageCommand.RaiseCanExecuteChanged();
            }
            catch(IOException exception)
            {
                StatusMessageText = Resources.MetaDataGUI.PackageCreationErrorStatusMessage;
                Log.Debug(Resources.MetaDataGUI.PackageCreationErrorLogMessage);

                string fileName = new DetailedExceptionMessage(exception).WriteToFile();

                if (!string.IsNullOrEmpty(fileName))
                    StatusMessagePath = string.Format(Resources.GUI.DetailedErrorMessageInfo, fileName);

                _isRunningCreatePackage = false;
            }
        }




        private int _GetNumNotDeletedEntriesIn(ObservableCollection<GuiMetaDataModel> collection)
        {
            return collection.Count(x => x.IsDeleted == false);
        }

        private bool _EnterElementIfOneNotDeletedEntryIsNotFilled(ObservableCollection<GuiMetaDataModel> collection, string entity, string contactPerson, string telephone, string email)
        {
            if ((collection.Count(x => (x.IsDeleted == false) && (x.Entity == string.Empty))) == 1)
            {
                foreach (var entry in collection)
                {
                    if (entry.IsDeleted == false)
                    {
                        entry.Entity = entity;
                        entry.ContactPerson = contactPerson;
                        entry.Telephone = telephone;
                        entry.Email = email;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
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
