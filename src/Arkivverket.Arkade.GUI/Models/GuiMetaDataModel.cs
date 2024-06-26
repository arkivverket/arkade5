using System;
using System.Windows;
using System.Windows.Input;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;
using Prism.Commands;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class GuiMetaDataModel : BindableBase
    {
        private Visibility _visibilityItem = Visibility.Visible;
        private Visibility _visibilityAddItem = Visibility.Hidden;
        private Visibility _deleteButtonVisibility = Visibility.Visible;

        public bool IsDeleted = false;

        private string _iconAdd = "PlusCircleOutline";
        private string _iconDelete = "Delete";
        private string _iconNameList = "MenuDown";

        public ICommand CommandDeleteItem { get; set; }
        public ICommand CommandAddItem { get; set; }
        public ICommand CommandNullOutEntry { get; set; }


        public string IconAdd
        {
            get => _iconAdd;
            set => SetProperty(ref _iconAdd, value);
        }

        public string IconDelete
        {
            get => _iconDelete;
            set => SetProperty(ref _iconDelete, value);
        }

        public string IconNameList
        {
            get => _iconNameList;
            set => SetProperty(ref _iconNameList, value);
        }

        public Visibility VisibilityItem
        {
            get => _visibilityItem;
            set => SetProperty(ref _visibilityItem, value);
        }

        public Visibility VisibilityAddItem
        {
            get => _visibilityAddItem;
            set => SetProperty(ref _visibilityAddItem, value);
        }


        public Visibility DeleteButtonVisibility
        {
            get => _deleteButtonVisibility;
            set => SetProperty(ref _deleteButtonVisibility, value);
        }


        private string _archiveDescription;
        private string _agreementNumber;
        private metsTypeMetsHdrRECORDSTATUS? _recordStatus;
        private string _projectName;
        private string _deliveryType;
        private string _packageNumber;
        private string _referenceCode;

        private string _entity;
        private string _contactPerson;
        private string _address;
        private string _telephone;
        private string _email;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Telephone
        {
            get => _telephone;
            set => SetProperty(ref _telephone, value);
        }

        public string ContactPerson
        {
            get => _contactPerson;
            set => SetProperty(ref _contactPerson, value);
        }

        public string Entity
        {
            get => _entity;
            set => SetProperty(ref _entity, value);
        }

        private string _systemName;
        private string _systemVersion;
        private string _systemType;
        private string _systemTypeVersion;

        public string SystemName
        {
            get => _systemName;
            set => SetProperty(ref _systemName, value);
        }

        public string SystemVersion
        {
            get => _systemVersion;
            set => SetProperty(ref _systemVersion, value);
        }

        public string SystemType
        {
            get => _systemType;
            set => SetProperty(ref _systemType, value);
        }

        public string SystemTypeVersion
        {
            get => _systemTypeVersion;
            set => SetProperty(ref _systemTypeVersion, value);
        }


        public string ArchiveDescription
        {
            get => _archiveDescription;
            set => SetProperty(ref _archiveDescription, value);
        }

        public string AgreementNumber
        {
            get => _agreementNumber;
            set => SetProperty(ref _agreementNumber, value);
        }

        public string RecordStatus
        {
            get => _recordStatus.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    SetProperty(ref _recordStatus, null);
                if (Enum.TryParse(value, out metsTypeMetsHdrRECORDSTATUS recordStatus))
                    SetProperty(ref _recordStatus, recordStatus);
                else
                    SetProperty(ref _recordStatus, null);
            }
        }

        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }

        public string DeliveryType
        {
            get => _deliveryType;
            set => SetProperty(ref _deliveryType, value);
        }

        public string PackageNumber
        {
            get => _packageNumber;
            set => SetProperty(ref _packageNumber, value);
        }

        public string ReferenceCode
        {
            get => _referenceCode;
            set => SetProperty(ref _referenceCode, value);
        }


        private DateTime? _startDate;
        private DateTime? _endDate;
        private DateTime? _extractionDate;
        private string _standardLabel;
        private string _userdefinedLabel;




        public DateTime? StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }


        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public DateTime? ExtractionDate
        {
            get => _extractionDate;
            set => SetProperty(ref _extractionDate, value);
        }

        public string StandardLabel
        {
            get => _standardLabel;
            set => SetProperty(ref _standardLabel, value);
        }

        public string UserdefinedLabel
        {
            get => _userdefinedLabel;
            set => SetProperty(ref _userdefinedLabel, value);
        }

        public GuiMetaDataModel(string archiveDescription, string agreementNumber, string recordStatus, string deliveryType,
            string projectName, string packageNumber, string referenceCode)
        {
            ArchiveDescription = archiveDescription;
            AgreementNumber = agreementNumber;
            RecordStatus = recordStatus;
            DeliveryType = deliveryType;
            ProjectName = projectName;
            PackageNumber = packageNumber;
            ReferenceCode = referenceCode;
            CommandDeleteItem = new DelegateCommand(ExecuteDeleteItem);
            CommandAddItem = new DelegateCommand(ExecuteAddItem);
            CommandNullOutEntry = new DelegateCommand(NullOutRecord);
        }


        public GuiMetaDataModel(string entity, string contactPerson, string address, string telephone, string email)
        {
            Entity = entity;
            ContactPerson = contactPerson;
            Address = address;
            Telephone = telephone;
            Email = email;

            CommandDeleteItem = new DelegateCommand(ExecuteDeleteItem);
            CommandAddItem = new DelegateCommand(ExecuteAddItem);
            CommandNullOutEntry = new DelegateCommand(NullOutRecord);
        }

        public GuiMetaDataModel(string systemName, string systemVersion, string systemType, string systemTypeVersion, GuiObjectType guiObjectType)
        {

            if (guiObjectType == GuiObjectType.system)
            {
                SystemName = systemName;
                SystemVersion = systemVersion;
                SystemType = systemType;
                SystemTypeVersion = systemTypeVersion;
            } 

            CommandDeleteItem = new DelegateCommand(ExecuteDeleteItem);
            CommandAddItem = new DelegateCommand(ExecuteAddItem);
            CommandNullOutEntry = new DelegateCommand(NullOutRecord);
        }


        public GuiMetaDataModel(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            CommandDeleteItem = new DelegateCommand(ExecuteDeleteItem);
            CommandAddItem = new DelegateCommand(ExecuteAddItem);
            CommandNullOutEntry = new DelegateCommand(NullOutRecord);
        }


        public GuiMetaDataModel(DateTime? extractionDate)
        {
            ExtractionDate = extractionDate;
            CommandDeleteItem = new DelegateCommand(ExecuteDeleteItem);
            CommandAddItem = new DelegateCommand(ExecuteAddItem);
            CommandNullOutEntry = new DelegateCommand(NullOutRecord);
        }


        public void ExecuteDeleteItem()
        {
            IsDeleted = true;
            VisibilityItem = Visibility.Collapsed;
            VisibilityAddItem = Visibility.Visible;
        }

        public void ExecuteAddItem()
        {
            IsDeleted = false;
            _ResetAllDataFields();
            VisibilityItem = Visibility.Visible;
            VisibilityAddItem = Visibility.Hidden;
        }


        public void SetDeleteButtonVisible()
        {
            DeleteButtonVisibility = Visibility.Visible;
        }

        public void SetDeleteButtonHidden()
        {
            DeleteButtonVisibility = Visibility.Hidden;
        }

        public void NullOutRecord()
        {
            _ResetAllDataFields();
        }


        private void _ResetAllDataFields()
        {
            Email = string.Empty;
            Address = string.Empty;
            Telephone = string.Empty;
            ContactPerson = string.Empty;
            Entity = string.Empty;
            SystemName = string.Empty;
            SystemVersion = string.Empty;
            SystemType = string.Empty;
            SystemTypeVersion = string.Empty;
            ArchiveDescription = string.Empty;
            AgreementNumber = string.Empty;
            RecordStatus = string.Empty;
            DeliveryType = string.Empty;
            ProjectName = string.Empty;
            PackageNumber = string.Empty;
            ReferenceCode = string.Empty;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            ExtractionDate = DateTime.Today;
        }
    }


    public enum GuiObjectType
    {
        archiveDescription,
        entity,
        system,
        archiveData,
        noarkObligatory
    }

}