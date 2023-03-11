using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class GroupsViewModel
    {
        public List<GroupViewModel> Groups { get; set; }
        public string MakeCode { get; internal set; }
        public string SubMakeCode { get; internal set; }
        public string ModelCode { get; internal set; }
        public string CatalogueCode { get; internal set; }
        public string ImagePath { get; internal set; }
        public List<GroupImageMapEntryViewModel> MapEntries { get; set; }
        public NavigationViewModel Navigation { get; set; }
        public List<CatalogueVariantsViewModel> ModelVariants { get; set; }
        public List<ModificationViewModel> Modifications { get; set; }
    }
}
