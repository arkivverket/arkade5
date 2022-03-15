namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class AddmlDefinitionFlatFileName
    {
        public string Path { get; }
        public string Name { get; }
        public string RelativeFilename { get; }
        public string FullFileName => System.IO.Path.Combine(Path, Name);

        public AddmlDefinitionFlatFileName(string path = "", string name = "", string relativeFilename="")
        {
            Path = path;
            Name = name;
            RelativeFilename = relativeFilename;
        }

        public override string ToString()
        {
            return FullFileName;
        }
    }
}