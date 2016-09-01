using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.LogInterface
{
    public interface ILogService
    {
        void Subscribe();


        event Action<LogEntry> LogMessageArrived;

    }
}
