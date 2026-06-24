using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class ArchiveInformationStatus : BindableBase
    {
        private string _archiveFileName;
        private string _archiveType;
        private string _informationPackageUuid;

        public void Update(string archiveFileName, string archiveType, string informationPackageUuid)
        {
            ArchiveFileName = archiveFileName;
            ArchiveType = archiveType;
            InformationPackageUuid = informationPackageUuid; // NB! UUID-transfer
        }

        public string ArchiveType
        {
            get => _archiveType;
            set => SetProperty(ref _archiveType, value);
        }

        public string InformationPackageUuid
        {
            get => _informationPackageUuid;
            set => SetProperty(ref _informationPackageUuid, value);
        }

        public string ArchiveFileName
        {
            get => _archiveFileName;
            set => SetProperty(ref _archiveFileName, value);
        }
    }
}