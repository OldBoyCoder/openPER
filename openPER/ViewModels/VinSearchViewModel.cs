using System.Collections.Generic;
using openPERModels;

namespace openPER.ViewModels
{
    public class VinSearchViewModel
    {
        public List<ModelModel> Models { get; set; }
        public MvsModel MvsData { get; set; }
        public string Organization { get; internal set; }
        public string ProductionDate { get; internal set; }
        public string EngineNumber { get; internal set; }
        public string ChassisNumber { get; set; }

    }
}
