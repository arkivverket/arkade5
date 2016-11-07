namespace Arkivverket.Arkade.Report
{
    public interface IReportGenerator<out T> where T: IReport
    {
        T Generate();

    }
}
