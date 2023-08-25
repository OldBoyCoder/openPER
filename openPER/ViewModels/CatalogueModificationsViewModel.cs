using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class CatalogueModificationsViewModel
    {
        public NavigationViewModel Navigation { get; set; }
        public List<ModifiedDrawingViewModel> ChangedDrawings { get; set; }
        public ModificationViewModel Details { get; set; }

    }
}
