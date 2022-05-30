using System.Collections.Generic;
using VinSearcher;

namespace openPER.Models
{
    public class VinSearchViewModel
    {
        public List<ModelViewModel> Models { get; set; }
        public MvsViewModel MvsData { get; set; }
        public string Organization { get; internal set; }
        public string ProductionDate { get; internal set; }
        public string EngineNumber { get; internal set; }
        public string ChassisNumber { get; set; }

    }
}
