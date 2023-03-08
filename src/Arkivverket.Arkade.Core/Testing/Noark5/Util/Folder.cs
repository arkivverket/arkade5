using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    internal abstract class Folder
    {
        public Folder ContainingFolder { get; set; }

        public ICollection<Folder> Folders { get; }
        public ICollection<Registration> Registrations { get; }

        public string Status { get; set; }

        protected Folder()
        {
            Folders = new Collection<Folder>();
            Registrations = new Collection<Registration>();
        }

        public bool Utgaar => Status?.ToLower() == "utgår";
    }
}
