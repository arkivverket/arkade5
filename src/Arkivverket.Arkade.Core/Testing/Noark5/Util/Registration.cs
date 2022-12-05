namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    internal class Registration
    {
        private readonly Folder _containingFolder;

        public string Status { get; set; }
        public string RegistrationId { get; set; }

        public Registration(Folder containingFolder)
        {
            _containingFolder = containingFolder;
        }

        public bool Utgaar()
        {
            if (Status?.ToLower() == "utgår")
                return true;

            return _containingFolder.Utgaar();
        }
    }
}
