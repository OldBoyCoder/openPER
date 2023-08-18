using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class DrawingsViewModel
    {
        public NavigationViewModel Navigation { get; set; }
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> Drawings { get; set; }
        public TableViewModel TableData { get; set; }
        public string Scope { get; set; }
        public List<SearchEngineViewModel> PartSearchUrl { get; internal set; }
    }
}
