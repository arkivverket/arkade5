using System.Windows;
using Arkivverket.Arkade.UI.Util;

namespace Arkivverket.Arkade.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LogConfiguration.ConfigureSeriLog();

            base.OnStartup(e);
            var bs = new Bootstrapper();
            bs.Run();
        }
    }
}