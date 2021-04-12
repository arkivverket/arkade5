using System;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.GUI.Languages;
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
            VersionInfoString = AboutGUI.VersionText + ArkadeVersion.Current;
            CopyrightInfoString = string.Format(AboutGUI.ArkadeCopyrightInformationText, DateTime.Now.Year);

            ShowLicenseWebPageCommand = new DelegateCommand(ShowLicenseWebPage);
            ShowSiegfriedWebPageCommand = new DelegateCommand(ShowSiegfriedWebPage);
            ShowApacheLicenseWebPageCommand = new DelegateCommand(ShowApacheLicenseWebPage);
        }

        private void ShowLicenseWebPage()
        {
            AboutGUI.GnuGpl3_0Uri.LaunchUrl();
        }

        private void ShowSiegfriedWebPage()
        {
            AboutGUI.SiegfriedUri.LaunchUrl();
        }

        private void ShowApacheLicenseWebPage()
        {
            AboutGUI.ApacheV2_0Uri.LaunchUrl();
        }
    }
}
