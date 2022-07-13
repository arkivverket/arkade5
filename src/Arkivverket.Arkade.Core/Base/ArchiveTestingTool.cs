namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveTestingTool
    {
        public string Name { get; }
        public string Version { get; }

        public ArchiveTestingTool(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }
}
