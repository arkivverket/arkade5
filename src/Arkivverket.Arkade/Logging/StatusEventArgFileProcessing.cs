using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventArgFileProcessing
    {
        public string FileName { get; set; }
        public string Status { get; set; }
        public int MetricCurrent { get; set; }
        public int MetricTotal { get; set; }

        public StatusEventArgFileProcessing(string fileName, string status, int metricCurrent, int metricTotal)
        {
            FileName = fileName;
            Status = status;
            MetricCurrent = metricCurrent;
            MetricTotal = metricTotal;
        }
    }
}
