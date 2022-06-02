using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class GroupsViewModel
    {
        public List<GroupModel> Groups { get; set; }
        public string CatalogueCode { get; internal set; }
    }
}
