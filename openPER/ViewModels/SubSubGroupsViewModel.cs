using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubSubGroupsViewModel : VersionedViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        public List<SubGroupViewModel> SubGroups { get; set; }
        public List<SubSubGroupViewModel> SubSubGroups { get; set; }
        public string MakeCode { get; set; }
        public string SubMakeCode { get; set; }
        public string ModelCode { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
        public int SubGroupCode { get; internal set; }

        public List<ModelViewModel> AllModels { get; set; }
        public List<CatalogueViewModel> AllCatalogues { get; set; }
        public List<GroupViewModel> AllGroups { get; set; }
        public List<SubGroupViewModel> AllSubGroups { get; set; }
    }
}
