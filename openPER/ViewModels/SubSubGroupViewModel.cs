using System.Collections.Generic;
using System.Linq;

namespace openPER.ViewModels
{
    public class SubSubGroupViewModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<ModificationViewModel> Modifications { get; set; }
        public List<VariationViewModel> Variations { get; set; }
        public bool Visible { get; set; } = true;
        public string Pattern { get; set; }
        public List<PatternViewModel> PatternParts { get; set; }

        public string ApplicabilityText
        {
            get
            {
                var rc = Pattern;
                if (Modifications != null)
                {
                    rc += " - " + string.Join(",", Modifications.Select(x => x.Type + x.Code.ToString("")));
                }
                return rc;
            }
        }
    }
}
