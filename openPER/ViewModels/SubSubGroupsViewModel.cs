using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubSubGroupsViewModel : VersionedViewModel
    {
        public List<SubGroupViewModel> SubGroups { get; set; }
        public List<SubSubGroupViewModel> SubSubGroups { get; set; }
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
        public int SubGroupCode { get; internal set; }
        public NavigationViewModel Navigation { get; set; }
    }
}
