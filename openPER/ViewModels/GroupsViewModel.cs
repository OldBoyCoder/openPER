using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class GroupsViewModel:VersionedViewModel
    {
        public List<GroupViewModel> Groups { get; set; }
        public string MakeCode { get; internal set; }
        public string ModelCode { get; internal set; }
        public string CatalogueCode { get; internal set; }
    }
}
