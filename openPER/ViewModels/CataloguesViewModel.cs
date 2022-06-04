using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class CataloguesViewModel : VersionedViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        public List<CatalogueViewModel> Catalogues { get; set; }

    }
}
