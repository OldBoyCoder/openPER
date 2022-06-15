using System.Collections.Generic;
using System.Security.AccessControl;

namespace openPER.ViewModels
{
    public class ClicheDrawingViewModel
    {
        public List<PartViewModel> Parts { get; set; }
        public double ParentPartNumber { get; set; } 
        public int CurrentDrawingNumber { get; set; }
    }
}
