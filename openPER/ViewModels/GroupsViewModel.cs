using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class GroupsViewModel:VersionedViewModel
    {
        public List<GroupViewModel> Groups { get; set; }
        public string CatalogueCode { get; internal set; }
    }
}
