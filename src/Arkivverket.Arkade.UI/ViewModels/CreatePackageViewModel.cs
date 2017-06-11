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
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Models;
using Arkivverket.Arkade.UI.Util;
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

        private readonly PopulateMetadataDataModels _populateMetadataDataModels = new PopulateMetadataDataModels();

        private GuiMetaDataModel _metaDataArchiveDescription = new GuiMetaDataModel(string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveCreators = new ObservableCollectionEx<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataTransferer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataProducer = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private ObservableCollection<GuiMetaDataModel> _metaDataOwners = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _metaDataRecipient = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty);
        private GuiMetaDataModel _metaDataSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, true);
        private GuiMetaDataModel _metaDataArchiveSystem = new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, true);
        private ObservableCollection<GuiMetaDataModel> _metaDataComments = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metadataPreregistreredUsers = new ObservableCollection<GuiMetaDataModel>();

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


        public ObservableCollection<GuiMetaDataModel> MetaDataPreregistreredUsers
        {
            get { return _metadataPreregistreredUsers; }
            set { SetProperty(ref _metadataPreregistreredUsers, value); }
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
                MetaDataProducer = _selectedProducerDataModel;
            }
        }

        public GuiMetaDataModel SelectedOwnerDataModel
        {
            get { return _selectedOwnerDataModel; }
            set
            {
                SetProperty(ref _selectedOwnerDataModel, value);
                MetaDataOwners.Add(_selectedOwnerDataModel);
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

            CreatePackageCommand = new DelegateCommand(RunCreatePackage, CanExecuteCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(RunNavigateToLoadArchivePage, CanExecuteCreatePackage);
            AddMetadataAchiveCreatorEntry = new DelegateCommand(RunAddMetadataAchiveCreatorEntry);
            AddMetadataAchiveOwnerEntry = new DelegateCommand(RunAddMetadataAchiveOwnerEntry);
            AddMetadataCommentEntry = new DelegateCommand(RunAddMetadataCommentEntry);

           ((INotifyPropertyChanged)MetaDataArchiveCreators).PropertyChanged += (x, y) => OnMetaDataArchiveCreatorsDataElementChaneChange();

        }

        public void OnMetaDataArchiveCreatorsDataElementChaneChange()
        {
            // Fires when any change is carried out in the MetaDataArchiveCreators ObservableCollection
            // Calls function to aminister GUI based as needed
            _SetDeleteButtonToHiddenIfCollectionOnlyContainsOneElements(MetaDataArchiveCreators);
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
            MetaDataComments.Add(new GuiMetaDataModel(string.Empty));
        }


        public void OnNavigatedTo(NavigationContext context)
        {
            _testSession = (TestSession) context.Parameters["TestSession"];

            FileInfo predefinedMetadataFieldValuesFileInfo = GetPredefinedMetadataFieldValuesFileInfo();

            if (predefinedMetadataFieldValuesFileInfo.Exists)
                LoadPredefinedMetadataFieldValues(predefinedMetadataFieldValuesFileInfo);
            else
                CreatePredefinedMetadataFieldValuesFile(predefinedMetadataFieldValuesFileInfo);

            _populateMetadataDataModels.DatafillArchiveEntity(_metaDataEntityInformationUnits, MetaDataPreregistreredUsers);

            // Pre populate metadata entries that require at least one entry
            RunAddMetadataAchiveCreatorEntry();

        }

        private static FileInfo GetPredefinedMetadataFieldValuesFileInfo()
        {
            string predefinedMetadataFieldValuesFileFullName = Path.Combine(
                ArkadeConstants.GetArkadeDirectory().FullName,
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
                    Entity = " ",
                    ContactPerson = " ",
                    Telephone = " ",
                    Email = " "
                },
                new MetadataEntityInformationUnit
                {
                    Entity = " ",
                    ContactPerson = " ",
                    Telephone = " ",
                    Email = " "
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
            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }

        private bool CanExecuteCreatePackage()
        {
            return !_isRunningCreatePackage && (SelectedPackageTypeSip || SelectedPackageTypeAip);
        }

        private void RunCreatePackage()
        {
            _testSession.ArchiveMetadata = new ArchiveMetadata
            {
                ArchiveDescription = ArchiveMetadataMapper.MapToArchiveDescription(_metaDataArchiveDescription),
                AgreementNumber = ArchiveMetadataMapper.MapToAgreementNumber(_metaDataArchiveDescription),
                ArchiveCreators = ArchiveMetadataMapper.MapToArchiveCreators(_metaDataArchiveCreators),
                Transferer = ArchiveMetadataMapper.MapToTransferer(_metaDataTransferer),
                Producer = ArchiveMetadataMapper.MapToProducer(_metaDataProducer),
                Owners = ArchiveMetadataMapper.MapToArchiveOwners(_metaDataOwners),
                Recipient = ArchiveMetadataMapper.MapToRecipient(_metaDataRecipient),
                System = ArchiveMetadataMapper.MapToSystem(_metaDataSystem),
                ArchiveSystem = ArchiveMetadataMapper.MapToArchiveSystem(_metaDataArchiveSystem),
                Comments = ArchiveMetadataMapper.MapToComments(_metaDataComments)
            };

            Log.Debug("Running create package command");
            _isRunningCreatePackage = true;
            CreatePackageCommand.RaiseCanExecuteChanged();

            PackageType packageType = SelectedPackageTypeSip
                ? PackageType.SubmissionInformationPackage
                : PackageType.ArchivalInformationPackage;

            // todo must be async
            _arkadeApi.CreatePackage(_testSession, packageType);

            string informationPackageFileName = _testSession.Archive.GetInformationPackageFileName().FullName;
            StatusMessageText = "IP og metadata lagret i ";
            StatusMessagePath = informationPackageFileName;
            Log.Debug("Package created in " + informationPackageFileName);

            _isRunningCreatePackage = false;
            //CreatePackageCommand.RaiseCanExecuteChanged();
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
