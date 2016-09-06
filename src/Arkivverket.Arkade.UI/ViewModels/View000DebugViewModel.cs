using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.LogInterface;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class View000DebugViewModel : BindableBase
    {

        public DelegateCommand DoSomethingCommand { get; set; }
        private readonly ILogService _logService;

        public View000DebugViewModel(ILogService logService)
        {
            DoSomethingCommand = new DelegateCommand(DoSomething);
            _logService = logService;
        }


        private void DoSomething()
        {
            Debug.Print("Issued the SoSomething command");
            _logService.Subscribe();
        }




    }
}
