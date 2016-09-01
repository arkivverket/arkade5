using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Arkivverket.Arkade.LogInterface
{
    public class RandomLogService : ILogService
    {

        private readonly object _symbolsLock = new object();
        private readonly Random _random = new Random();
        private readonly DispatcherTimer _timer;
        private const int TimerInterval = 1000;

        private readonly List<string>  _logGobbledigook = new List<string>(new string[] 
        {"Arkivet er pakket ut (svada)",
         "Filen hadde en fin skjekksum (svada)",
         "XML filen er jammen lang (svada)",
         "Arkiv dokumentet validerer ikke (svada)",
         "Dokumentet finne ikke (svada)",
         "Vi fant et ukjent tegnsett (svada)"
        });


        public void Subscribe()
        {

        }

        public event Action<LogEntry> LogMessageArrived;


        public RandomLogService()
        {
            _timer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(TimerInterval)
            };

            _timer.Tick += delegate
            {
                lock (_symbolsLock)
                {
                    Array values = Enum.GetValues(typeof(LogLevel));
                    LogLevel randomLogLevel = (LogLevel) values.GetValue(_random.Next(values.Length));

                    values = Enum.GetValues(typeof(LogSubsystem));
                    LogSubsystem randomSubsystem = (LogSubsystem) values.GetValue(_random.Next(values.Length));

                    String logMessage = _logGobbledigook[_random.Next(_logGobbledigook.Count)];

                    LogEntry logEntry = new LogEntry(logMessage, randomSubsystem, randomLogLevel);

                    // TODO: Check this code bit out
                    Debug.Assert(LogMessageArrived != null, "LogMessageArrived != null");
                    LogMessageArrived(logEntry);
                }
            };
            _timer.Start();
        }

    }
}
