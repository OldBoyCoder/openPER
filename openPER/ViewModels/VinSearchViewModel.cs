using System.Collections.Generic;
using openPERModels;

namespace openPER.ViewModels
{
    public class VinSearchViewModel
    {
        public List<ModelModel> Models { get; set; }
        public List<MvsDataModel> MvsData { get; set; }
        public string Organization { get; internal set; }
        public string ProductionDate { get; internal set; }
        public string EngineNumber { get; internal set; }
        public string ChassisNumber { get; set; }
        public string InteriorColourCode { get; set; }
        public string InteriorColourDesc { get; set; }

        public string ExteriorColourCode { get; set; }
        public string ExteriorColourDesc { get; set; }
        public List<VinSearchOptionsViewModel> Options { get; set; }

    }
    public class VinSearchOptionsViewModel
    {
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string CodeDescription { get; set; }
        public string ValueDescription { get; set; }

    }
}
