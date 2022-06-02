using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class GroupsViewModel
    {
        public List<GroupViewModel> Groups { get; set; }
        public string CatalogueCode { get; internal set; }
    }
}
