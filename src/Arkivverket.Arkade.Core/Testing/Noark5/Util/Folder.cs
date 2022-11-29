namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    internal class Folder
    {
        public Folder ContainingFolder { get; }
        public string Status { get; set; }

        public Folder(Folder containingFolder)
        {
            ContainingFolder = containingFolder;
        }

        public bool Utgaar()
        {
            if (Status?.ToLower() == "utgår")
                return true;

            return ContainingFolder?.Utgaar() ?? false;
        }
    }
}
