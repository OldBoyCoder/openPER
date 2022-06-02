using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubGroupsViewModel:VersionedViewModel
    {
        public List<SubGroupViewModel> SubGroups { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
    }
}
