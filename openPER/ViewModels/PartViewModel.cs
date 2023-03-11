using System.Collections.Generic;

namespace openPER.ViewModels
{

    public class PartViewModel
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string FurtherDescription { get; set; }
        public string Quantity { get; set; }
        public int TableOrder { get; set; }
        public string Notes { get; set; }
        public string Notes1 { get; set; }
        public string Notes2 { get; set; }
        public string Notes3 { get; set; }
        public string Sequence { get; set; }
        public List<ModificationViewModel> Modifications { get; set; }
        public string Compatibility { get; internal set; }
        public bool IsAComponent { get; set; }
        public List<PartReplacementViewModel> Replaces { get; set; }
        public List<PartReplacementViewModel> ReplacedBy { get; set; }
        public bool Visible { get; set; } = true;
        public string Colour { get; set; }


    }
}