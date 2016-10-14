namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFileReader : IFlatFileReader
    {
        public bool HasMoreRecords()
        {
            return false;
        }

        public Record GetNextRecord()
        {
            return null;
        }
    }
}
