using Arkivverket.Arkade.GUI.Views;
using Prism.Modularity;
using Prism.Regions;

namespace Arkivverket.Arkade.GUI
{
    public class ModuleAModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleAModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainContentRegion", typeof(LoadArchiveExtraction));
        }
    }
}