using System.Windows;
using Prism.Regions;

namespace Arkivverket.Arkade.UI.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = Properties.Resources.General_WindowTitle;
        }
    }
}
