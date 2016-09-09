using Arkivverket.Arkade.UI.Properties;
using Microsoft.Win32;

namespace Arkivverket.Arkade.UI.Util
{
    public class FileFolderDialogs
    {
        public string ChooseFile(string title, string defaultExtension, string filter)
        {
            string filename;

            var dlg = new OpenFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = filter
            };

            var status = dlg.ShowDialog();

            if (status == null)
            {
                // No file was chosen
                throw new UiPprocessingExceptions(Resources.FileException_NoFileChosen);
            }
            if (status == false)
            {
                // File chooser returned false ... some issue?    
                throw new UiPprocessingExceptions(Resources.FileException_ErrorWithChosenFile);
            }
            if (status == true)
            {
                filename = dlg.FileName;
            }
            else
            {
                // File chooser returned false ... some issue?    
                throw new UiPprocessingExceptions(Resources.FileException_GeneralError);
            }
            return filename;
        }
    }
}