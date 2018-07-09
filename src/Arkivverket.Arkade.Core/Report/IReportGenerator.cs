using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Report
{
    public interface IReportGenerator
    {
        void Generate(TestSession testSession);
    }
}