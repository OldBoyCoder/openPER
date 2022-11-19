using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class ClicheViewModel 
    {

        public BreadcrumbViewModel Breadcrumb { get; set; }

        // We need a list of all drawings for this page
        public List<DrawingKeyViewModel> ClicheDrawings { get; set; }
        public int CurrentDrawing { get; set; }

        public ClicheDrawingViewModel CurrentClicheDrawing { get; set; }
    }
}
