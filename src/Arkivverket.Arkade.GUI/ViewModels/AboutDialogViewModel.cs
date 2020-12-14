using System;
using System.Diagnostics;
using Arkivverket.Arkade.Core.Util;
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
            Process.Start(new ProcessStartInfo
            {
                FileName = Resources.AboutGUI.GnuGpl3_0Uri,
                UseShellExecute = true
            });
        }

        private void ShowSiegfriedWebPage()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Resources.AboutGUI.SiegfriedUri,
                UseShellExecute = true
            });
        }

        private void ShowApacheLicenseWebPage()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Resources.AboutGUI.ApacheV2_0Uri,
                UseShellExecute = true
            });
        }
    }
}
