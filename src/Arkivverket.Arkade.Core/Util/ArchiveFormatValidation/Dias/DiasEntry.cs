namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public abstract record DiasEntry
    {
        public readonly string Name;

        public abstract bool ExistsAtPath(string path);

        protected DiasEntry(string name)
        {
            Name = name;
        }
    }
}
