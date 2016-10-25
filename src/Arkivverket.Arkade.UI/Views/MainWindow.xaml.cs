using Arkivverket.Arkade.Util;
using System.Windows;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = string.Format(UI.Resources.UI.General_WindowTitle, ArkadeVersion.Version);
        }
    }
}