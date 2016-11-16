using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Report
{
    public interface IReportGenerator
    {
        IReport Generate(TestSession testSession);
    }
}