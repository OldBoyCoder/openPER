using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class ClicheDrawingViewModel
    {
        public List<PartViewModel> Parts { get; set; }
        public decimal ParentPartNumber { get; set; } 
        public int CurrentDrawingNumber { get; set; }
    }
}
