using System.IO;

namespace Arkivverket.Arkade.Core.Report
{
    public interface IReportGenerator
    {
        void Generate(TestReport testReport, Stream stream);
    }
}