using System;
using System.Windows.Controls;
using Arkivverket.Arkade.GUI.Util;

namespace Arkivverket.Arkade.GUI.Views
{
    public partial class LoadArchiveExtraction : UserControl
    {
        public LoadArchiveExtraction()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                new DetailedExceptionMessage(e).ShowMessageBox();
                throw;
            }
        }
    }
}