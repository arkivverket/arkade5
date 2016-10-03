using System.Reflection;
using System.Windows;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = string.Format(UI.Resources.UI.General_WindowTitle, "0.3.0"); // Todo - get correct application version from assembly
        }
    }
}