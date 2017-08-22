using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommandMain { get; set; }
        public DelegateCommand ShowUserGuideCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommandMain = new DelegateCommand<string>(Navigate);
            ShowUserGuideCommand = new DelegateCommand(ShowUserGuide);
        }

        private void ShowUserGuide()
        {
            System.Diagnostics.Process.Start("http://arkade.arkitektum.no/no/latest/Brukerveiledning.html");
        }


        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("MainContentRegion", uri);
        }
    }
}
