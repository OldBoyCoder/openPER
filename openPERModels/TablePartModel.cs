using System.Collections.Generic;

namespace openPERModels
{
    public class TablePartModel
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string FurtherDescription { get; set; }
        public string Quantity { get; set; }
        public int TableOrder { get; set; }
        public string Notes { get; set; }
        public int Sequence { get; set; }
        public List<ModificationModel> Modifications { get; set; }
        public string Compatibility { get; set; }
        public bool IsAComponent { get; set; }
        public string Colour { get; set; }
        public List<ColourModel> Colours { get; set; }
        public List<PartHotspotModel> Hotspots { get; set; }
        public string RecondPartNumber { get; set; }
        public string WreckPartNumber { get; set; }
    }
}