using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.UI.Util;

namespace Arkivverket.Arkade.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LogConfiguration.ConfigureSeriLog();

            base.OnStartup(e);
            Bootstrapper bs = new Bootstrapper();
            bs.Run();
        }
    }
}
