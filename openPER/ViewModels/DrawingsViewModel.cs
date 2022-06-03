using System.Collections.Generic;
using openPER.Models;

namespace openPER.ViewModels
{
    public class DrawingsViewModel : VersionedViewModel
    {
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> Drawings { get; set; }
        public TableViewModel TableData { get; set; }
    }
}
