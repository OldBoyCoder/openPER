using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class SubGroupsViewModel
    {
        public List<SubGroupViewModel> SubGroups { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
    }
}
