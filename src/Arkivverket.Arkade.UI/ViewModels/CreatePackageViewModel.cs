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
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveCreator = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataEntity = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _selectedEntityDataModel;

        private ObservableCollection<GuiMetaDataModel> _metaDatatransferer = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _selectedTransfererDataModel;

        private ObservableCollection<GuiMetaDataModel> _metaDataProducer = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _selectedProducerDataModel;

        private ObservableCollection<GuiMetaDataModel> _metaDataOwner = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _selectedOwnerDataModel;

        private ObservableCollection<GuiMetaDataModel> _metaDataRecipient = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataSystem = new ObservableCollection<GuiMetaDataModel>();
        private ObservableCollection<GuiMetaDataModel> _metaDataArchiveSystem = new ObservableCollection<GuiMetaDataModel>();
        private GuiMetaDataModel _selectedArchiveSystemDataModel;

        private ObservableCollection<GuiMetaDataModel> _metaDataComments = new ObservableCollection<GuiMetaDataModel>();


        private List<MetadataEntityInformationUnit> _metaDataEntityInformationUnits = new List<MetadataEntityInformationUnit>();
        public DelegateCommand CreatePackageCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }
        public DelegateCommand AddMetadataAchiveDescriptionEntry { get; set; }
        public DelegateCommand AddMetadataAchiveCreatorEntry { get; set; }
        public DelegateCommand AddMetadataAchiveTransfererEntry { get; set; }
        public DelegateCommand AddMetadataAchiveProducerEntry { get; set; }
        public DelegateCommand AddMetadataAchiveOwnerEntry { get; set; }
        public DelegateCommand AddMetadataAchiveArchiveSystemEntry { get; set; }
        public DelegateCommand AddMetadataCommentEntry { get; set; }

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


        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveEntity
        {
            get { return _metaDataEntity; }
            set { SetProperty(ref _metaDataEntity, value); }
        }

        public GuiMetaDataModel SelectedEntityDataModel
        {
            get { return _selectedEntityDataModel; }
            set
            {
                SetProperty(ref _selectedEntityDataModel, value);
                MetaDataArchiveCreator.Add(_selectedEntityDataModel);
            }
        }
        public ObservableCollection<GuiMetaDataModel> MetaDataTransferer
        {
            get { return _metaDatatransferer; }
            set { SetProperty(ref _metaDatatransferer, value); }
        }

        public GuiMetaDataModel SelectedTransfererDataModel
        {
            get { return _selectedTransfererDataModel; }
            set
            {
                SetProperty(ref _selectedTransfererDataModel, value);
                MetaDataTransferer.Add(_selectedTransfererDataModel);
            }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataProducer
        {
            get { return _metaDataProducer; }
            set { SetProperty(ref _metaDataProducer, value); }
        }

        public GuiMetaDataModel SelectedProducerDataModel
        {
            get { return _selectedProducerDataModel; }
            set
            {
                SetProperty(ref _selectedProducerDataModel, value);
                MetaDataProducer.Add(_selectedProducerDataModel);
            }
        }


        public ObservableCollection<GuiMetaDataModel> MetaDataOwner
        {
            get { return _metaDataOwner; }
            set { SetProperty(ref _metaDataOwner, value); }
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


        public GuiMetaDataModel SelectedOwnerDataModel
        {
            get { return _selectedOwnerDataModel; }
            set
            {
                SetProperty(ref _selectedOwnerDataModel, value);
                MetaDataOwner.Add(_selectedOwnerDataModel);
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

        public ObservableCollection<GuiMetaDataModel> MetaDataModelArchiveDescriptions
        {
            get { return _metaDataArchiveDescriptions; }
            set { SetProperty(ref _metaDataArchiveDescriptions, value); }
        }

        public ObservableCollection<GuiMetaDataModel> MetaDataArchiveCreator
        {
            get { return _metaDataArchiveCreator; }
            set { SetProperty(ref _metaDataArchiveCreator, value); }
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
            MetaDataArchiveCreator.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveTransfererEntry()
        {
            MetaDataTransferer.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveProducerEntry()
        {
            MetaDataProducer.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        public void RunAddMetadataAchiveOwnerEntry()
        {
            MetaDataOwner.Add(new GuiMetaDataModel(string.Empty, string.Empty, string.Empty, string.Empty));
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

            //_populateMetadataDataModels.DatafillArchiveDescription(_testSession.ArchiveMetadata, MetaDataModelArchiveDescriptions);
            //_populateMetadataDataModels.DatafillArchiveCreator(_testSession.ArchiveMetadata, MetaDataArchiveCreator);
            _populateMetadataDataModels.DatafillArchiveEntity(_metaDataEntityInformationUnits, MetaDataArchiveEntity);
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
