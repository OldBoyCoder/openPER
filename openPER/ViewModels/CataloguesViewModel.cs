using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class CataloguesViewModel : VersionedViewModel
    {
        public List<ModelViewModel> Models { get; set; }
        public List<CatalogueViewModel> Catalogues { get; set; }
        public NavigationViewModel Navigation { get; set; }

    }
}
