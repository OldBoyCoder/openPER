using System.Collections.Generic;

namespace openPER.ViewModels
{
    public class SubSubGroupViewModel
    {
        public int Code { get; set; }
        public List<ModificationViewModel> Modifications { get; set; }
        public List<VariationViewModel> Variations { get; set; }
        public List<OptionViewModel> Options { get; set; }

    }
}
