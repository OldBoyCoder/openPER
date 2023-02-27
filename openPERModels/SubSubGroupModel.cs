using System.Collections.Generic;

namespace openPERModels
{
    public class SubSubGroupModel
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<ModificationModel> Modifications { get; set; }
        public List<VariationModel> Variations { get; set; }
        public List<OptionModel> Options { get; set; }
        public string Pattern { get; set; }
    }
}
