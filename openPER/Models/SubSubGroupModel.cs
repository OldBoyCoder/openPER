using System.Collections.Generic;
using openPER.ViewModels;

namespace openPER.Models
{
    public class SubSubGroupModel
    {
        public int Code { get; set; }
        public List<ModificationModel> Modifications { get; set; }
        public List<VariationModel> Variations { get; set; }
        public List<OptionModel> Options { get; set; }
    }
}
