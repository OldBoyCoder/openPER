using System.Collections.Generic;
using openPER.Models;
namespace openPER.ViewModels
{
    public class CataloguesViewModel : VersionedViewModel
    {
        public List<CatalogueViewModel> Catalogues { get; set; }

    }
}
