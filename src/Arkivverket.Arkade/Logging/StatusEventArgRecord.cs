using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Logging
{
    public class StatusEventArgRecord
    {
        public string EventName { get; set; }
        public string Status { get; set; }
        public int Metric { get; set; }

        public StatusEventArgRecord(string eventName, string status, int metric)
        {
            EventName = eventName;
            Status = status;
            Metric = metric;
        }
    }
}
