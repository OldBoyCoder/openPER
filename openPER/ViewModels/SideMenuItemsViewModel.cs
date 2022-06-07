using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SideMenuItemsViewModel
    {
        public List<MakeViewModel> AllMakes { get; set; }
        public List<ModelViewModel> AllModels { get; set; }
        public List<CatalogueViewModel> AllCatalogues { get; set; }
        public List<GroupViewModel> AllGroups { get; set; }
        public List<SubGroupViewModel> AllSubGroups { get; set; }
        public List<SubSubGroupViewModel> AllSubSubGroups { get; set; }

    }
}