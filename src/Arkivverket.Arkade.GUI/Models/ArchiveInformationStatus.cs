using System;
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
            Uuid = archiveInformationEvent.Uuid;
        }

        public string ArchiveType
        {
            get { return _archiveType; }
            set { SetProperty(ref _archiveType, value); }
        }

        public string Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public string ArchiveFileName
        {
            get { return _archiveFileName; }
            set { SetProperty(ref _archiveFileName, value); }
        }
    }
}