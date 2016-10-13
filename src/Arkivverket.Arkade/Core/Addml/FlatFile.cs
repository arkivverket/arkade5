namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFile
    {
        public AddmlFlatFileDefinition Definition { private set; get; }

        public FlatFile(AddmlFlatFileDefinition definition)
        {
            Definition = definition;
        }
    }
}