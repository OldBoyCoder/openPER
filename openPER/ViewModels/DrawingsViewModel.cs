using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class DrawingsViewModel : VersionedViewModel
    {
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> Drawings { get; set; }
    }
}
