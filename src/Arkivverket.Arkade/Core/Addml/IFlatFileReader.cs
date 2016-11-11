namespace Arkivverket.Arkade.Core.Addml
{
    // TODO jostein: Rewrite to extend IEnumerator<Record>

    public interface IFlatFileReader
    {
        bool HasMoreRecords();
        Record GetNextRecord();
    }
}