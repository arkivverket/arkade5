using System.IO;

namespace Arkivverket.Arkade.Core.Report
{
    public interface IReport
    {
        void Save(FileInfo file);
    }
}