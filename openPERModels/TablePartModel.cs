using System.Collections.Generic;

namespace openPERModels
{
    public class TablePartModel
    {
        public decimal PartNumber { get; set; }
        public string Description { get; set; }
        public string FurtherDescription { get; set; }
        public string Quantity { get; set; }
        public int TableOrder { get; set; }
        public string Notes { get; set; }
        public string Notes1 { get; set; }
        public string Notes2 { get; set; }
        public string Notes3 { get; set; }
        public string Sequence { get; set; }
        public List<ModificationModel> Modifications { get; set; }
        public string Compatibility { get; set; }
        public bool IsAComponent { get; set; }
    }
}