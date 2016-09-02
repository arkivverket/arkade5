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

        public View000DebugViewModel()
        {
            DoSomethingCommand = new DelegateCommand(DoSomething);
        }


        private void DoSomething()
        {
            Debug.Print("Issued the SoSomething command");
        }




    }
}
