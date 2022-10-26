using System.IO;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class DiasFile : DiasEntry
    {
        public DiasFile(string fileName) : base(fileName)
        {
        }

        public override bool ExistsAtPath(string path)
        {
            return File.Exists(Path.Combine(path, Name));
        }
    }
}
