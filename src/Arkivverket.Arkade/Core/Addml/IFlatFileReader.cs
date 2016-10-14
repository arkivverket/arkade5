namespace Arkivverket.Arkade.Core.Addml
{
    public interface IFlatFileReader
    {
        bool HasMoreRecords();
        Record GetNextRecord();
    }
}
