using System.Collections.Generic;
using openPERModels;

namespace openPER.ViewModels
{
    public class DrawingsViewModel
    {
        public NavigationViewModel Navigation { get; set; }
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> Drawings { get; set; }
        public TableViewModel TableData { get; set; }
        public DrawingsScope Scope { get; set; }
    }
}
