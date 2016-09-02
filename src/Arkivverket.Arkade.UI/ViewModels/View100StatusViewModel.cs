using System;
using System.Collections.Generic;
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

        //private readonly ILogService _logService;


        public View100StatusViewModel()
        {
            //_logService = logService;
            //_logService.LogMessageArrived += LogMessageArrived;
        }

        private void LogMessageArrived(LogEntry obj)
        {
            Debug.Print("We got something: " + obj.Message);
        }
    }
}
