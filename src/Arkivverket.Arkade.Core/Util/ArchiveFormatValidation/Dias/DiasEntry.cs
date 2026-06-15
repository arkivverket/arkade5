namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    // Deliberately a record: DIAS entries are equated by kind + Name, so a directory's entry set
    // dedupes same-named entries (a DiasFile("dias-mets.xml") is "the same expected file" however it
    // was constructed). The DiasProvider templates — and the structure simulation feeding
    // ArkadeCoreApiTest's expected file lists — rely on this dedupe. These records are mutable
    // builders, so do not use `with`: it would shallow-copy and share the mutable entry set.
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
