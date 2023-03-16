using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Html;

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
        public List<ColourViewModel> Colours { get; set; } = new();

        public List<PartHotspotViewModel> Hotspots { get; set; }

        public string FullDescription => $"{Description} {FurtherDescription}";
        public string CompatibilityTooltip { get; set; }

        public string SearchLink(string makeDescription)
        {
            var rc =
                $"<a href=\"https://www.google.com/search?q={makeDescription}+{PartNumber}\" target=\"_blank\" class=\"bi bi-search\"></a>";

            return rc;
        }
    }

}