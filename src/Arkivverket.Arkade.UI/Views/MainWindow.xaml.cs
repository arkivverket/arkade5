using System.Reflection;
using System.Windows;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = string.Format(UI.Resources.UI.General_WindowTitle, typeof(App).Assembly.GetName().Version);
        }
    }
}