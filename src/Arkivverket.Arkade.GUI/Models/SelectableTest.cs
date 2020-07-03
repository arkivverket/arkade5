using Arkivverket.Arkade.Core.Util;
using Prism.Mvvm;

namespace Arkivverket.Arkade.GUI.Models
{
    public class SelectableTest : BindableBase
    {
        public TestId TestId { get; set; }
        public string DisplayName { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
