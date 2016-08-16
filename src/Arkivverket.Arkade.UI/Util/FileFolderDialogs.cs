using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.UI.Util
{
    public class FileFolderDialogs
    {

        public string ChooseFile(string title, string defaultExtension, string filter)
        {
            string filename;

            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = filter
            };

            Nullable<bool> status = dlg.ShowDialog();

            if (status == null)
            {
                // No file was chosen
                throw new UiPprocessingExceptions(Properties.Resources.FileException_NoFileChosen);
            } else if (status == false)
            {
                // File chooser returned false ... some issue?    
                throw new UiPprocessingExceptions(Properties.Resources.FileException_ErrorWithChosenFile);
            }
            else if (status == true)
            {
                filename = dlg.FileName;
            } else
            {
                // File chooser returned false ... some issue?    
                throw new UiPprocessingExceptions(Properties.Resources.FileException_GeneralError);
            }
            return filename;
        }


    }
}
