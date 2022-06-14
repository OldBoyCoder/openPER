using System.Collections.Generic;

namespace openPER.ViewModels
{
    public enum DrawingsRequestType{
        Catalogue,
        Group,
        SubGroup,
        SubSubGroup,
    }
    public class DrawingsViewModel : VersionedViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }
        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> Drawings { get; set; }
        public TableViewModel TableData { get; set; }
        public DrawingsRequestType RequestType { get; set; }
    }
}
