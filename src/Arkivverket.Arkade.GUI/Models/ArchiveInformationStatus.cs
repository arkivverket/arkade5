using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class ArchiveInformationStatus : BindableBase
    {
        private string _archiveFileName;
        private string _archiveType;
        private string _uuid;

        public void Update(string archiveFileName, string archiveType, string uuid)
        {
            ArchiveFileName = archiveFileName;
            ArchiveType = archiveType;
            Uuid = uuid; // NB! UUID-transfer
        }

        public string ArchiveType
        {
            get => _archiveType;
            set => SetProperty(ref _archiveType, value);
        }

        public string Uuid
        {
            get => _uuid;
            set => SetProperty(ref _uuid, value);
        }

        public string ArchiveFileName
        {
            get => _archiveFileName;
            set => SetProperty(ref _archiveFileName, value);
        }
    }
}