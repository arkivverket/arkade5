using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Report
{
    public interface IReportGenerator
    {
        void Generate(TestSession testSession);
    }
}