using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class ModelsViewModel:VersionedViewModel
    {
        public List<ModelViewModel> Models{ get; set; }

    }
}

