using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Report
{
    public class PdfReport : IReport
    {
        private byte[] _pdf;

        public PdfReport(byte[] pdf)
        {
            _pdf = pdf;
        }
    }
}
