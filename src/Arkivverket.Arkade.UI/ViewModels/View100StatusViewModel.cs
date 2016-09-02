using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.LogInterface;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.ViewModels
{
    class View100StatusViewModel : BindableBase
    {

        private readonly ILogService _logService;

        private string _logString;
        public string LogString
        {
            get { return _logString; }
            set { SetProperty(ref _logString, value); }
        }

        public View100StatusViewModel(ILogService logService)
        {
            _logService = logService;
            _logService.LogMessageArrived += LogMessageArrived;
        }

        private void LogMessageArrived(LogEntry obj)
        {
            string msg = $"{obj.Timestamp.ToShortDateString()}: {obj.Level.ToString()} {obj.Subsystem.ToString()}: {obj.Message}";
            LogString = $"{LogString}\n{msg}";
            Debug.Print(msg);
        }
    }
}
