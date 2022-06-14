using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class ClicheViewModel : VersionedViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> ClicheDrawings { get; set; }
        //public TableViewModel TableData { get; set; }
        //public DrawingsRequestType RequestType { get; set; }
    }
}
