using System.IO;

namespace Arkivverket.Arkade.Core.Report
{
    public class PdfReport : IReport
    {
        private readonly byte[] _pdf;

        public PdfReport(byte[] pdf)
        {
            _pdf = pdf;
        }

        public void Save(FileInfo file)
        {
            File.WriteAllBytes(file.FullName, _pdf);
        }

        public byte[] ToBytes()
        {
            return _pdf;
        }
    }
}