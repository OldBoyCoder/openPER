using System.Collections.Generic;

namespace openPERModels
{
    public class PartModel
    {
        public double PartNumber { get; set; }
        public string Description { get; set; }
        public string FamilyCode { get; set; }
        public string FamilyDescription { get; set; }
        public string UnitOfSale { get; set; }
        public int Weight { get; set; }
        public List<PartDrawing> Drawings { get; set; }
    }
}
