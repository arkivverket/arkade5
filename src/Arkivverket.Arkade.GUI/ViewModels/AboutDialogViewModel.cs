using System;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.GUI.Util;
using Prism.Commands;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class AboutDialogViewModel : BindableBase
    {
        public string VersionInfoString { get; }
        public string CopyrightInfoString { get; }
        public DelegateCommand ShowLicenseWebPageCommand { get; set; }
        public DelegateCommand ShowSiegfriedWebPageCommand { get; set; }
        public DelegateCommand ShowApacheLicenseWebPageCommand { get; set; }

        public AboutDialogViewModel()
        {
            VersionInfoString = "Versjon: " + ArkadeVersion.Current;
            CopyrightInfoString = string.Format(Resources.AboutGUI.ArkadeCopyrightInformationText, DateTime.Now.Year);

            ShowLicenseWebPageCommand = new DelegateCommand(ShowLicenseWebPage);
            ShowSiegfriedWebPageCommand = new DelegateCommand(ShowSiegfriedWebPage);
            ShowApacheLicenseWebPageCommand = new DelegateCommand(ShowApacheLicenseWebPage);
        }

        private void ShowLicenseWebPage()
        {
            Resources.AboutGUI.GnuGpl3_0Uri.LaunchUrl();
        }

        private void ShowSiegfriedWebPage()
        {
            Resources.AboutGUI.SiegfriedUri.LaunchUrl();
        }

        private void ShowApacheLicenseWebPage()
        {
            Resources.AboutGUI.ApacheV2_0Uri.LaunchUrl();
        }
    }
}
