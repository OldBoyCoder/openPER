using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class SubSubGroupsViewModel : VersionedViewModel
    {
        public List<SubSubGroupViewModel> SubSubGroups { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
        public int SubGroupCode { get; internal set; }
    }
}
