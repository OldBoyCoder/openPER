using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubGroupsViewModel:VersionedViewModel
    {
        public List<SubGroupViewModel> SubGroups { get; set; }

        public string MakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
    }
}
