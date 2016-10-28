using System;
using System.Windows.Controls;
using Arkivverket.Arkade.UI.Util;

namespace Arkivverket.Arkade.UI.Views
{
    public partial class TestRunner : UserControl
    {
        public TestRunner()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
                throw;
            }
        }
    }
}
