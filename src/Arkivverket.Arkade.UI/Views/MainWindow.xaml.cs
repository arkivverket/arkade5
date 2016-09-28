using System.Windows;

namespace Arkivverket.Arkade.UI.Views
{
    /// <summary>
    ///     Interaction logic for Shell.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = Arkivverket.Arkade.UI.Resources.UI.General_WindowTitle;
        }
    }
}