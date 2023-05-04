using Arkivverket.Arkade.Core.Logging;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class ArchiveInformationStatus : BindableBase
    {
        private string _archiveFileName;
        private string _archiveType;
        private string _uuid;

        public void Update(ArchiveInformationEventArgs archiveInformationEvent)
        {
            ArchiveFileName = archiveInformationEvent.ArchiveFileName;
            ArchiveType = archiveInformationEvent.ArchiveType;
            Uuid = archiveInformationEvent.Uuid; // NB! UUID-transfer
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