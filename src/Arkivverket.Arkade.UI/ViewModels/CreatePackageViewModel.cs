using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Models;
using Arkivverket.Arkade.Util;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using static System.String;

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

        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveDescriptions = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveCreators = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDatatransferers = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataProducers = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataOwners = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataRecipient = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataSystem = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveSystem = new ObservableCollection<GuiMetaDataModel>();
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
        public DelegateCommand AddMetadataAchiveDescriptionEntry { get; set; }
        public DelegateCommand AddMetadataAchiveCreatorEntry { get; set; }
        public DelegateCommand AddMetadataAchiveTransfererEntry { get; set; }
        public DelegateCommand AddMetadataAchiveProducerEntry { get; set; }
        public DelegateCommand AddMetadataAchiveOwnerEntry { get; set; }
        public DelegateCommand AddMetadataAchiveArchiveSystemEntry { get; set; }
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


        public ObservableCollection<GuiMetaDataModel> MetaDataModelArchiveDescriptions
        {
            get { return _metaDataArchiveDescriptions; }
            set { SetProperty(ref _metaDataArchiveDescriptions, value); }
        }


        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveCreators
        {
            get { return _metaDataArchiveCreators; }
            set { SetProperty(ref _metaDataArchiveCreators, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataTransferers
        {
            get { return _metaDatatransferers; }
            set { SetProperty(ref _metaDatatransferers, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataProducers
        {
            get { return _metaDataProducers; }
            set { SetProperty(ref _metaDataProducers, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataOwners
        {
            get { return _metaDataOwners; }
            set { SetProperty(ref _metaDataOwners, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataRecipient
        {
            get { return _metaDataRecipient; }
            set { SetProperty(ref _metaDataRecipient, value); }
        }
        public ObservableCollection<GuiMetaDataModel> MetaDataSystem
        {
            get { return _metaDataSystem; }
            set { SetProperty(ref _metaDataSystem, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveSystem
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
                MetaDataTransferers.Add(_selectedTransfererDataModel);
            }
        }

        public GuiMetaDataModel SelectedProducerDataModel
        {
            get { return _selectedProducerDataModel; }
            set
            {
                SetProperty(ref _selectedProducerDataModel, value);
                MetaDataProducers.Add(_selectedProducerDataModel);
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
                MetaDataArchiveSystem.Add(_selectedArchiveSystemDataModel);
            }
        }

        public GuiMetaDataModel SelectedCreatorDataModel
        {
            get { return _selectedCreatorDataModel; }
            set
            {
                SetProperty(ref _selectedCreatorDataModel, value);
                MetaDataArchiveCreators.Add(_selectedCreatorDataModel);
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
            AddMetadataAchiveDescriptionEntry = new DelegateCommand(RunAddMetadataAchiveDescriptionEntry);
            AddMetadataAchiveCreatorEntry = new DelegateCommand(RunAddMetadataAchiveCreatorEntry);
            AddMetadataAchiveTransfererEntry = new DelegateCommand(RunAddMetadataAchiveTransfererEntry);
            AddMetadataAchiveProducerEntry = new DelegateCommand(RunAddMetadataAchiveProducerEntry);
            AddMetadataAchiveOwnerEntry = new DelegateCommand(RunAddMetadataAchiveOwnerEntry);
            AddMetadataAchiveArchiveSystemEntry = new DelegateCommand(RunAddMetadataAchiveArchiveSystemEntry);
            AddMetadataCommentEntry = new DelegateCommand(RunAddMetadataCommentEntry);
        }


        public void RunAddMetadataCommentEntry()
        {
            MetaDataComments.Add(new GuiMetaDataModel(string.Empty));
        }

        public void RunAddMetadataAchiveDescriptionEntry()
        {
            MetaDataModelArchiveDescriptions.Add(new GuiMetaDataModel(string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveCreatorEntry()
        {
            MetaDataArchiveCreators.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveTransfererEntry()
        {
            MetaDataTransferers.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveProducerEntry()
        {
            MetaDataProducers.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveOwnerEntry()
        {
            MetaDataOwners.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveArchiveSystemEntry()
        {
            MetaDataArchiveSystem.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty, true));
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
            _populateMetadataDataModels.DatafillArchiveRecipient(_testSession.ArchiveMetadata, MetaDataRecipient);
            _populateMetadataDataModels.DatafillArchiveSystem(_testSession.ArchiveMetadata, MetaDataSystem);

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
    }
}
