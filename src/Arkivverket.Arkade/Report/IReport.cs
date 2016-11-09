using System.IO;

namespace Arkivverket.Arkade.Report
{
    public interface IReport
    {
        void Save(FileInfo file);
    }
}