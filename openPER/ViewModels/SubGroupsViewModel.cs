using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class SubGroupsViewModel
    {
        public List<SubGroupModel> SubGroups { get; set; }
        public string CatalogueCode { get; set; }
        public int GroupCode { get; internal set; }
    }
}
