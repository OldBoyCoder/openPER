using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class ModelsViewModel:VersionedViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        public List<ModelViewModel> Models{ get; set; }
        public string MakeCode { get; set; }

    }
}

